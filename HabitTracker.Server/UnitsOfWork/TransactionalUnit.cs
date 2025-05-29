using HabitTracker.Server.Storage;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.UnitsOfWork
{
    public abstract class TransactionalUnit<T>
    {
        public ITransaction Transaction;
        protected readonly ILogger logger;

        public TransactionalUnit(IStorage storage, ILogger logger)
        {
            Transaction = storage.StartTransaction();
            this.logger = logger;
        }

        protected abstract T? Work();

        public T? Execute()
        {
            try
            {
                logger.LogInformation("TransactionalUnit - Execute - attempting work");
                T? result = Work();

                if (result != null)
                {
                    logger.LogInformation("TransactionalUnit - Execute - committing");
                    Transaction.Commit();
                    return result;
                }
                logger.LogInformation("TransactionalUnit - Execute - rolling back");
                Transaction.Rollback();
                return default;
            }
            catch (UnauthorizedException ex)
            {
                Transaction.Rollback();
                throw new UnauthorizedException(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                Transaction.Rollback();
                throw new ForbiddenException(ex.Message);
            }
            catch (ConflictException ex)
            {
                Transaction.Rollback();
                throw new ConflictException(ex.Message);
            }
            catch (BadRequestException ex)
            {
                Transaction.Rollback();
                throw new BadRequestException(ex.Message);
            }
            catch (NotFoundException ex)
            {
                Transaction.Rollback();
                throw new NotFoundException(ex.Message);
            }
            catch (AppException ex)
            {
                Transaction.Rollback();
                throw new AppException(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogInformation("TransactionalUnit - Execute - caught error rolling back {@Ex}", ex);
                Transaction.Rollback();
                return default;
            }
            finally
            {
                logger.LogInformation("TransactionalUnit - Execute - disposing");
                Transaction.Dispose();
            }
        }


    }
}
