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
    public class RegistradoControllerTests
    {
        [TestMethod]
        public void DetailsTestsNotNull(string correo)
        {
            RegistradoController registrado = new RegistradoController();
            ViewResult result = registrado.Details(correo) as ViewResult;
            Assert.IsNotNull(result);
        }
        [TestMethod]

        public void ValidarRegistroView(FormCollection form)
        {
            RegistradoController registrado = new RegistradoController();
            ViewResult result = registrado.ValidarRegistro(form) as ViewResult;
            Assert.AreEqual("ValidarRegistro", result.ViewName);
        }
    }
}
