using System;
using System.Globalization;

namespace Nut.MediatR.Test;

public static class TestHelper
{
    public static IDisposable SetEnglishCulture()
    {
        return new CultureSwitcher(CultureInfo.GetCultureInfo("en"));
    }

    private class CultureSwitcher : IDisposable
    {
        private CultureInfo _sourceCulture = null;
        public CultureSwitcher(CultureInfo targetCulture)
        {
            _sourceCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = targetCulture;
            CultureInfo.CurrentUICulture = targetCulture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _sourceCulture;
            CultureInfo.CurrentUICulture = _sourceCulture;
        }
    }
}
