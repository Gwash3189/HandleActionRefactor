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
            return new CustResult<TINPUT>(inputModel, this.Invoker);
        }
    }

    public class CustResult<TINPUT> : ActionResult
    {
        public TINPUT inputModel { get; set; }
        public IInvoker Invoker { get; set; }


        public CustResult(TINPUT inputModel, IInvoker invoker)
        {
            this.inputModel = inputModel;
            this.Invoker = invoker;
        }

        public CustResult<TINPUT, TRET> Returning<TRET>()
        {

            return new CustResult<TINPUT, TRET>(this.inputModel, this.Invoker);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CustResult<TINPUT, TRET> : ActionResult
    {
        public TINPUT inputModel { get; set; }
        public IInvoker Invoker { get; set; }
        public Func<TRET, ActionResult> onSuccess;
        public Func< ActionResult> onError;

        public CustResult(TINPUT inputModel, IInvoker invoker)
        {
            this.inputModel = inputModel;
            this.Invoker = invoker;
        }

        public CustResult<TINPUT, TRET> On(Func<TRET, bool> inputFunction, Func<TRET, ActionResult> stuff)
        {
            return this;
        }

        public CustResult<TINPUT, TRET> OnSuccess(Func<TRET, ActionResult> inputFunction)
        {
            this.onSuccess = inputFunction;
            return this;
        }

        public CustResult<TINPUT, TRET> OnError(Func<ActionResult> inputFunction)
        {
            this.onError = inputFunction;
            return this;
        }

        public override void ExecuteResult(ControllerContext context)
        {   
            //on success do this
            var result = Invoker.Execute<TRET>(this.inputModel);
            var actionresult = onSuccess(result);
            actionresult.ExecuteResult(context);
            
        }
    }
}