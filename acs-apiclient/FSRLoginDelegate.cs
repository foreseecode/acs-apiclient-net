using System;
using System.Collections.Generic;
using AcsApi.Models;

namespace AcsApi
{
    public interface FSRLoginDelegate
    {
        void ShowIdentityProviderSelector(List<IdentityProvider> identityProviders);

        void ShouldBeginPasswordFlow();

        void ShouldBeginExternalFlow();
    }
}
