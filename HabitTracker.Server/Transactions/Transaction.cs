using HabitTracker.Server.UnitsOfWork;

namespace HabitTracker.Server.Transactions
{
    public class TransactionResult {
        private List<UnitOfWorkResult> UnitOfWorkResults { get; }
        public TransactionResult() 
        {
            UnitOfWorkResults = new List<UnitOfWorkResult>();
        }

        public void AddUnitOfWorkResult(UnitOfWorkResult unitOfWorkResult)
        {
            UnitOfWorkResults.Add(unitOfWorkResult);
        }
    }

    public abstract class Transaction
    {
        protected readonly List<IUnitOfWork> UnitsOfWork;

        public Transaction(List<IUnitOfWork> unitsOfWork)
        {
            UnitsOfWork = unitsOfWork;
        }

        public async Task<TransactionResult> Execute()
        {
            int lastSuccessfulIndex = -1;
            TransactionResult transactionResult = new TransactionResult();

            for (var i = 0; i < UnitsOfWork.Count; i++)
            {
                UnitOfWorkResult result = await UnitsOfWork[i].execute();
                transactionResult.AddUnitOfWorkResult(result);

                if (!result.Success)
                {
                    if (lastSuccessfulIndex >= 0) 
                    {
                        await Rollback(lastSuccessfulIndex);
                    }
                    return transactionResult;
                }

                lastSuccessfulIndex++;
            }

            return transactionResult;
        }

        public async Task<TransactionResult> Rollback(int lastSuccessfulIndex)
        {
            TransactionResult transactionResult = new TransactionResult();

            for (var i = 0; i < lastSuccessfulIndex; i++)
            {
                UnitOfWorkResult result = await UnitsOfWork[i].rollback();
                transactionResult.AddUnitOfWorkResult(result);

                if (!result.Success)
                {
                    throw new Exception("Failed to perform rollback");
                }
            }

            return transactionResult;
        }

    }
}
