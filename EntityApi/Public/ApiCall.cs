using System;
using System.Collections.Generic;
using EntityApi.Private;
using EntityApi.Public.EventArgs;

namespace EntityApi.Public
{
    public class ApiCall
    {
        internal ApiService Api => ApiService.GetInstance();
        internal ApiCallConfiguration Configuration;
        
        protected Action FinalAction;
        protected Action BeforeAction;

        protected ApiCall() { }

        internal ApiCall(ApiCallConfiguration configuration)
        {
            Configuration = configuration;
            SuccessActions = new List<Action<ApiSuccesArgs>>();
            FailActions = new List<Action<ApiFailArgs>>();
        }


        public List<Action<ApiSuccesArgs>> SuccessActions;
        public void NotifySuccess(ApiSuccesArgs args)
        {
            foreach (var action in SuccessActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public List<Action<ApiFailArgs>> FailActions;
        public void NotifyFailure(ApiFailArgs args)
        {
            foreach (var action in FailActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public ApiCall OnSuccess(Action<ApiSuccesArgs> onSuccesAction)
        {
            SuccessActions.Add(onSuccesAction);
            return this;
        }

        public ApiCall OnFail(Action<ApiFailArgs> onFailAction)
        {
            FailActions.Add(onFailAction);
            return this;
        }

        public ApiCall Finally(Action finalAction)
        {
            FinalAction = finalAction;
            return this;
        }

        public ApiCall Before(Action beforeAction)
        {
            BeforeAction = beforeAction;
            return this;
        }

        public virtual void Call()
        {
            BeforeAction?.Invoke();
            Api.SendRequest(this);
        }

        internal void CallPriority()
        {
            BeforeAction?.Invoke();
            Api.SendRequest(this,true);
        }
    }

    public class ApiCall<T> : ApiCall
    {
        internal ApiCall( ApiCallConfiguration configuration) 
        {
            Configuration = configuration;
        }

        public new List<Action<ApiSuccesArgs<T>>> SuccessActions;
        public virtual void NotifySuccess(ApiSuccesArgs<T> args)
        {
            foreach (var action in SuccessActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public virtual ApiCall<T> OnSuccess(Action<ApiSuccesArgs<T>> onSuccesAction)
        {
           SuccessActions.Add(onSuccesAction);
            return this;
        }

        public new virtual ApiCall<T> OnFail(Action<ApiFailArgs> onFailAction)
        {
            FailActions.Add(onFailAction);
            return this;
        }

        public new virtual ApiCall<T> Finally(Action finalAction)
        {
            FinalAction = finalAction;
            return this;
        }

        public new virtual ApiCall<T> Before(Action beforeAction)
        {
            BeforeAction = beforeAction;
            return this;
        }

        public new virtual void Call()
        {
            BeforeAction?.Invoke();
            Api.SendRequest(this);
        }
    }
}
