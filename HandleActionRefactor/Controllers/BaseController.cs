using System.Web.Mvc;
using SchoStack.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
        public IInvoker Invoker { get; set; }

        public CustResult<TInputValue> Handle<TInputValue>(TInputValue inputModel)
        {
            return new CustResult<TInputValue>(inputModel, this.Invoker);
        }
    }

    public class CustResult<TInputValue> : ActionResult
    {
        public TInputValue InputModel { get; set; }
        public IInvoker Invoker { get; set; }
        private Func<ActionResult> OnSuccessFunction;
        private Func<ActionResult> OnErrorFunction;


        public CustResult(TInputValue inputModel, IInvoker invoker)
        {
            this.InputModel = inputModel;
            this.Invoker = invoker;
        }

        public CustResult<TInputValue> OnError(Func<ActionResult> onerror)
        {
            this.OnErrorFunction = onerror;
            return this;

        }

        public CustResult<TInputValue> OnSuccess(Func<ActionResult> onsuccess)
        {
            this.OnSuccessFunction = onsuccess;
            return this;

        }

        public CustResult<TInputValue, TRET> Returning<TRET>()
        {
            if (this.OnErrorFunction != null)
            {
                return new CustResult<TInputValue, TRET>(this.InputModel, this.Invoker, this.OnErrorFunction);
            }
            else
            {
                return new CustResult<TInputValue, TRET>(this.InputModel, this.Invoker);
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!context.Controller.ViewData.ModelState.IsValid)
            {
                var actionresult = this.OnErrorFunction();
                actionresult.ExecuteResult(context);
            }
            else
            {
                Invoker.Execute(this.InputModel);
                var actionresult = OnSuccessFunction();
                actionresult.ExecuteResult(context);
            }
        }
    }























    public class CustResult<TInputValue, TReturnValue> : ActionResult
    {
        private TInputValue InputMode { get; set; }
        private IInvoker Invoker { get; set; }
        private Func<TReturnValue, ActionResult> OnSuccessFunction;
        private Func<ActionResult> OnErrorFunction;
        private List<ResultBag<TReturnValue>> ResultBagList;

        public CustResult(TInputValue inputModel, IInvoker invoker)
        {
            this.InputMode = inputModel;
            this.Invoker = invoker;
            this.ResultBagList = new List<ResultBag<TReturnValue>>();
        }
        
        public CustResult(TInputValue inputModel, IInvoker invoker, Func<ActionResult> OnErrorFunction)
        {
            this.InputMode = inputModel;
            this.Invoker = invoker;
            this.ResultBagList = new List<ResultBag<TReturnValue>>();
            this.OnErrorFunction = OnErrorFunction;
        }

        public CustResult<TInputValue, TReturnValue> On(Func<TReturnValue, bool> propertyFunction, Func<TReturnValue, ActionResult> inputFunction)
        {
            this.ResultBagList.Add(new ResultBag<TReturnValue>(propertyFunction, inputFunction));
            return this;
        }

        public CustResult<TInputValue, TReturnValue> OnSuccess(Func<TReturnValue, ActionResult> inputFunction)
        {
            this.OnSuccessFunction = inputFunction;
            return this;
        }

        public CustResult<TInputValue, TReturnValue> OnError(Func<ActionResult> inputFunction)
        {
            this.OnErrorFunction = inputFunction;
            return this;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!context.Controller.ViewData.ModelState.IsValid)
            {
                var actionresult = OnErrorFunction();
                actionresult.ExecuteResult(context);
            }
            else
            {
                var result = Invoker.Execute<TReturnValue>(this.InputMode);

                var q = this.ResultBagList.FirstOrDefault(x => x.OnTrue(result));
                if (q != null)
                {
                    q.ExecuteIfTrue(result).ExecuteResult(context);
                    return;
                }

                var actionresult = OnSuccessFunction(result);
                actionresult.ExecuteResult(context);
            }
        }
    }

    public class ResultBag<T>
    {
        public Func<T, bool> OnTrue { get; set; }
        public Func<T, ActionResult> ExecuteIfTrue { get; set; }

        public ResultBag(Func<T, bool> ontruefunction, Func<T, ActionResult> executeiftruefunction)
        {
            this.OnTrue = ontruefunction;
            this.ExecuteIfTrue = executeiftruefunction;
        }
    }
}