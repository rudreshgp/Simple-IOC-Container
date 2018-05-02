using SimpleIOCContainer.IServices;

namespace SimpleIOCContainer.Services
{


    public class Test : ITest
    {
        private ITest1 _test1;
        public Test(ITest1 test1, ITest2 test2, ITest1 test3)
        {
            _test1 = test1;
        }
    }


    public class Test1 : ITest1
    {

    }

    public class Test2 : ITest1, ITest2
    {

    }
}
