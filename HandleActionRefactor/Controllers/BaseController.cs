using System.Web.Mvc;
using SchoStack.Web;
using System;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
        public IInvoker Invoker { get; set; }

        public CustResult<TINPUT> Handle<TINPUT>(TINPUT inputModel)
        {
            return new CustResult<TINPUT>(inputModel);
        }
    }

    public class CustResult <TINPUT> : ActionResult
    {
        public TINPUT inputModel { get; set; }

        public CustResult(TINPUT inputModel)
        {
            this.inputModel = inputModel;
        }

        public CustResult<TINPUT, TRET> Returning<TRET>() 
        {

            return new CustResult<TINPUT, TRET>(this.inputModel);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CustResult<TINPUT, TRET> : ActionResult
    {
        public TINPUT inputModel { get; set; }

        public CustResult(TINPUT inputModel)
        {
            this.inputModel = inputModel;
        }

        public CustResult<TINPUT, TRET> On(Func<TRET, bool> inputFunction, Func<TRET, ActionResult> stuff)
        {
            return this;
        }

        public CustResult<TINPUT, TRET> OnSuccess(Func<TRET, ActionResult> inputFunction)
        {
            return this;
        }

        public CustResult<TINPUT, TRET> OnError(Func<ActionResult> inputFunction)
        {
            return this;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}