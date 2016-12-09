using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MovieModel;


namespace ModelsTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()                                       // horror movie at 10:30
        {
            Movie m = new Movie();
            m.Genre = Genre.Horror;
            DateTime testtime = new DateTime(2015, 12, 12, 22, 30, 00);
            Assert.AreEqual(m.ShowTime, testtime.TimeOfDay);
        }

        [TestMethod]
        public void TestMethod2()                                       // test datetime relatively:
        {                                                               // Assert true if both propositions are true
            Movie m = new Movie();                                      // or both are false
            m.Genre = Genre.Horror;
            TimeSpan testtime = DateTime.Now.TimeOfDay;
            Assert.AreEqual(m.MovieNow(m.ShowTime).Contains("later today"), 
                testtime < m.ShowTime);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Movie m = new Movie();
            m.Genre = Genre.Horror;
            TimeSpan testtime = DateTime.Now.TimeOfDay;                       // testtime = right now
            Assert.AreEqual(m.MovieNow(m.ShowTime).Contains("tomorrow"),
                testtime > m.ShowTime);                                       // if test time is after the show starts,
        }                                                                     // returned string should contain "tomorrow"

        [TestMethod]
        public void TestMethod4()
        {
            Movie m = new Movie();
            m.Genre = Genre.Horror;
            TimeSpan testtime = DateTime.Now.TimeOfDay;
            Assert.IsTrue(m.MovieNow(testtime).Contains("now"));                // we can pass a datetime into the method, so 
        }                                                                       // we know now is always DateTime.Now

        [TestMethod]
        public void TestMethod5()
        {
            Movie m = new Movie();
            m.Genre = Genre.Comedy;
            Assert.IsTrue(m.MovieNow(m.ShowTime).Contains("18:30:00"));         
        }

        [TestMethod]
        public void TestMethod6()
        {
            Movie m = new Movie();                                              // there is no genre number nine 9...
            m.Genre = (Genre)9;                                                 // but enums are not type-safe, so
            Assert.IsTrue(m.MovieNow(m.ShowTime).Contains("00:00:00"));         // switch and else should have default error case 
        }


        //[TestMethod]
        //public void TestMethod0()                                       // always fail
        //{
        //    Assert.Fail();
        //}
    }
}
