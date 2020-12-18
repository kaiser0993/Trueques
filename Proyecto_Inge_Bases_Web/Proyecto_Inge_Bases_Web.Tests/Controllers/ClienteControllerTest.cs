using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proyecto_Inge_Bases_Web;
using Proyecto_Inge_Bases_Web.Controllers;
using Proyecto_Inge_Bases_Web.Models;
using Proyecto_Integrador_Bases_Inge.Controllers;

namespace Proyecto_Inge_Bases_Web.Tests.Controllers
{
    [TestClass]
    public class ClienteControllerTest
    {
        [TestMethod]
        public void Index()
        {
            ClienteController cliente = new ClienteController();
            ViewResult result = cliente.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
        [TestMethod]

        public void TestListaAmigos()
        {
            ClienteController cliente = new ClienteController();
            ViewResult result = cliente.ListaAmigos() as ViewResult;
            Assert.AreEqual("ListaAmigos", result.ViewName);
        }
        [TestMethod]
        public void TestEdit(string id)
        {
            ClienteController cliente = new ClienteController();
            ViewResult result = (ViewResult)cliente.Edit(id);
            Assert.AreEqual("Edit", result.ViewName);
        }




    }
}
