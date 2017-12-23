using System;
namespace AcsApi
{
	public partial class LoginController
	{
        public override void BeginLoginFlow<T>(T context, Models.IdentityProvider identityProvider)
        {
            
        }
        
        protected override void RunOnMainThread(Action action)
        {
            throw new NotImplementedException();
        }
        
        protected override void RunOnBackgroundThread(Action action)
        {
            throw new NotImplementedException();
        }
	}
}
