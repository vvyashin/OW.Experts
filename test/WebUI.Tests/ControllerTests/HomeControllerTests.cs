using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebUI.Controllers;
using WebUI.Infrastructure;

namespace WebUI.UnitTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController CreateControllerUnderTest()
        {
            return new HomeController();
        }

        [Test]
        public void Index_UserIsAdmin_RedirectToAdminIndex()
        {
            ICurrentUser stubCurrentUser = Substitute.For<ICurrentUser>();
            stubCurrentUser.IsAdmin.Returns(true);
            HomeController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = stubCurrentUser;
            
            var result = (RedirectToRouteResult) cut.Index();

            Assert.AreEqual("Admin", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Index_UserIsExpertAndNotAdmin_RedirectToExpertIndex()
        {
            ICurrentUser stubCurrentUser = Substitute.For<ICurrentUser>();
            stubCurrentUser.IsAdmin.Returns(false);
            stubCurrentUser.IsExpert.Returns(true);
            HomeController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = stubCurrentUser;
            
            var result = (RedirectToRouteResult) cut.Index();
            
            Assert.AreEqual("Expert", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Index_UserIsGuest_RedirectToAccountRegister()
        {
            ICurrentUser stubCurrentUser = Substitute.For<ICurrentUser>();
            stubCurrentUser.IsAdmin.Returns(false);
            stubCurrentUser.IsExpert.Returns(false);
            HomeController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = stubCurrentUser;
            
            var result = (RedirectToRouteResult) cut.Index();
            
            Assert.AreEqual("Account", result.RouteValues["controller"]);
            Assert.AreEqual("Register", result.RouteValues["action"]);            
        }
    }
}