using ConsoleActorBenchmark.Messages;
using Proto;
using Proto.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    internal class TransferProcess : IActor
    {
        private readonly decimal _amount;
        private readonly double _availability;
        private readonly Behavior _behavior = new();
        private readonly PID _from;
        private readonly Persistence _persistence;
        private readonly string _persistenceId;
        private readonly Random _random;
        private readonly PID _to;
        private bool _processCompleted;
        private bool _restarting;
        private bool _stopping;

        public TransferProcess(
             PID from,
             PID to,
             decimal amount,
             IProvider provider,
             string persistenceId,
             Random random,
             double availability
         )
        {
            _from = from;
            _to = to;
            _amount = amount;
            _persistenceId = persistenceId;
            _random = random;
            _availability = availability;
            _persistence = Persistence.WithEventSourcing(provider, persistenceId, ApplyEvent);
        }

        public async Task ReceiveAsync(IContext context)
        {
            var message = context.Message;

            switch (message)
            {
                case Started:
                    _behavior.Become(Starting);
                    await _persistence.RecoverStateAsync();

                    break;
                case Stopping:
                    _stopping = true;

                    break;
                case Restarting:
                    _restarting = true;

                    break;
                case Stopped _ when !_processCompleted:
                    await _persistence.PersistEventAsync(new TransferFailed("Unknown. Transfer Process crashed"));

                    await _persistence.PersistEventAsync(
                        new EscalateTransfer("Unknown failure. Transfer Process crashed")
                    );

                    context.Send(context.Parent!, new UnknownResult(context.Self));

                    return;
                case Terminated _ when _restarting || _stopping:
                    return;

                default:
                    // simulate failures of the transfer process itself
                    if (Fail())
                    {
                        throw new Exception();
                    }

                    break;
            }

            await _behavior.ReceiveAsync(context);
        }


        private static Props TryCredit(PID targetActor, decimal amount) =>
     Props
         .FromProducer(() => new AccountProxy(targetActor, sender => new Credit(amount, sender)));

        private static Props TryDebit(PID targetActor, decimal amount) =>
            Props
                .FromProducer(() => new AccountProxy(targetActor, sender => new Debit(amount, sender)));

        private void ApplyEvent(Event @event)
        {
            Console.WriteLine($"Applying event: {@event.Data}");

            switch (@event.Data)
            {
                case TransferStarted:
                    _behavior.Become(AwaitingDebitConfirmation);

                    break;
                case AccountDebited:
                    _behavior.Become(AwaitingCreditConfirmation);

                    break;
                case CreditRefused:
                    _behavior.Become(RollingBackDebit);

                    break;
                case AccountCredited:
                case DebitRolledBack:
                case TransferFailed:
                    _processCompleted = true;

                    break;
            }
        }

        private bool Fail()
        {
            var comparison = _random.NextDouble() * 100;

            return comparison > _availability;
        }




        private async Task Starting(IContext context)
        {
            if (context.Message is Started)
            {
                context.SpawnNamed(TryDebit(_from, -_amount), "DebitAttempt");
                _behavior.Become(AwaitingDebitConfirmation);
            }
        }



        private async Task AwaitingDebitConfirmation(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.SpawnNamed(TryDebit(_from, -_amount), "DebitAttempt");

                    break;
                case OK _:
                    await _persistence.PersistEventAsync(new AccountDebited());
                    context.SpawnNamed(TryCredit(_to, +_amount), "CreditAttempt");

                    break;
                case Refused _:
                    await _persistence.PersistEventAsync(new TransferFailed("Debit refused"));
                    context.Send(context.Parent!, new Result.FailedButConsistentResult(context.Self));
                    StopAll(context);

                    break;
                case Terminated _:
                    await _persistence.PersistEventAsync(new StatusUnknown());
                    StopAll(context);

                    break;
            }
        }


        private async Task AwaitingCreditConfirmation(IContext context)
        {
            switch (context.Message)
            {
                case Started:
                    context.SpawnNamed(TryCredit(_to, +_amount), "CreditAttempt");
                    break;

                case OK:
                    var fromBalance =
                        await context.RequestAsync<decimal>(_from, new GetBalance(), TimeSpan.FromMilliseconds(2000));

                    var toBalance =
                        await context.RequestAsync<decimal>(_to, new GetBalance(), TimeSpan.FromMilliseconds(2000));

                    await _persistence.PersistEventAsync(new AccountCredited());
                    await _persistence.PersistEventAsync(new TransferCompleted(_from, fromBalance, _to, toBalance));
                    context.Send(context.Parent!, new Result.SuccessResult(context.Self));
                    StopAll(context);
                    break;

                case Refused:
                    await _persistence.PersistEventAsync(new CreditRefused());
                    context.SpawnNamed(TryCredit(_from, +_amount), "RollbackDebit");
                    break;

                case Terminated:
                    await _persistence.PersistEventAsync(new StatusUnknown());
                    StopAll(context);
                    break;
            }
        }

        private async Task RollingBackDebit(IContext context)
        {
            switch (context.Message)
            {
                case Started:
                    context.SpawnNamed(TryCredit(_from, +_amount), "RollbackDebit");

                    break;
                case OK:
                    await _persistence.PersistEventAsync(new DebitRolledBack());
                    await _persistence.PersistEventAsync(new TransferFailed($"Unable to rollback debit to {_to.Id}"));
                    context.Send(context.Parent!, new Result.FailedAndInconsistent(context.Self));
                    StopAll(context);

                    break;
                case Refused: 
                case Terminated:
                    await _persistence.PersistEventAsync(
                        new TransferFailed($"Unable to rollback process. {_from.Id} is owed {_amount}")
                    );

                    await _persistence.PersistEventAsync(new EscalateTransfer($"{_from.Id} is owed {_amount}"));
                    context.Send(context.Parent!, new Result.FailedAndInconsistent(context.Self));
                    StopAll(context);

                    break;
            }
        }

        private void StopAll(IContext context)
        {
            context.Stop(_from);
            context.Stop(_to);
            context.Stop(context.Self);
        }



    }
}
