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

        protected ApiCall()
        {
            SuccessActions = new List<Action<ApiResponseArgs>>();
            FailActions = new List<Action<ApiResponseArgs>>();
        }

        internal ApiCall(ApiCallConfiguration configuration) : this()
        {
            Configuration = configuration;
            
        }


        public List<Action<ApiResponseArgs>> SuccessActions;
        public void NotifySuccess(ApiResponseArgs args)
        {
            foreach (var action in SuccessActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public List<Action<ApiResponseArgs>> FailActions;
        public void NotifyFailure(ApiResponseArgs args)
        {
            foreach (var action in FailActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public ApiCall OnSuccess(Action<ApiResponseArgs> onSuccesAction)
        {
            SuccessActions.Add(onSuccesAction);
            return this;
        }

        public ApiCall OnFail(Action<ApiResponseArgs> onFailAction)
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
        internal ApiCall( ApiCallConfiguration configuration) : base()
        {
            Configuration = configuration;
            SuccessActions = new List<Action<ApiResponseArgs<T>>>();
        }

        public List<Action<ApiResponseArgs<T>>> SuccessActions;
        public virtual void NotifySuccess(ApiResponseArgs<T> args)
        {
            foreach (var action in SuccessActions)
            {
                action(args);
            }
            FinalAction?.Invoke();
        }

        public virtual ApiCall<T> OnSuccess(Action<ApiResponseArgs<T>> onSuccesAction)
        {
            SuccessActions.Add(onSuccesAction);
            return this;
        }

        public new virtual ApiCall<T> OnFail(Action<ApiResponseArgs> onFailAction)
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
