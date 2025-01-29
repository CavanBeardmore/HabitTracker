using HabitTracker.Server.UnitsOfWork;

//namespace habittracker.server.transactions
//{
//    public class transactionresult
//    {
//        private list<unitofworkresult> unitofworkresults { get; }
//        public transactionresult()
//        {
//            unitofworkresults = new list<unitofworkresult>();
//        }

//        public void addunitofworkresult(unitofworkresult unitofworkresult)
//        {
//            unitofworkresults.add(unitofworkresult);
//        }
//    }

//    public abstract class transaction
//    {
//        protected readonly list<iunitofwork> unitsofwork;

//        public transaction(list<iunitofwork> unitsofwork)
//        {
//            unitsofwork = unitsofwork;
//        }

//        public async task<transactionresult> execute()
//        {
//            int lastsuccessfulindex = -1;
//            transactionresult transactionresult = new transactionresult();

//            for (var i = 0; i < unitsofwork.count; i++)
//            {
//                unitofworkresult result = await unitsofwork[i].execute();
//                transactionresult.addunitofworkresult(result);

//                if (!result.success)
//                {
//                    if (lastsuccessfulindex >= 0)
//                    {
//                        await rollback(lastsuccessfulindex);
//                    }
//                    return transactionresult;
//                }

//                lastsuccessfulindex++;
//            }

//            return transactionresult;
//        }

//        public async task<transactionresult> rollback(int lastsuccessfulindex)
//        {
//            transactionresult transactionresult = new transactionresult();

//            for (var i = 0; i < lastsuccessfulindex; i++)
//            {
//                unitofworkresult result = await unitsofwork[i].rollback();
//                transactionresult.addunitofworkresult(result);

//                if (!result.success)
//                {
//                    throw new exception("failed to perform rollback");
//                }
//            }

//            return transactionresult;
//        }

//    }
//}
