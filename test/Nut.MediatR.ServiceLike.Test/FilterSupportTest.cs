using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class FilterSupportTest
{
    [Fact]
    public void IsValidTIlterTypeAllCore_fileterTypesがnullの場合は例外が発生する()
    {
        Action act = () => FilterSupport.IsValidFilterTypeAllCore(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsValidTIlterTypeAllCore_fileterTypesにnullが含まれる場合はfalseが返る()
    {
        var result = FilterSupport.IsValidFilterTypeAllCore(
            new Type[] { typeof(Filter1), null, typeof(Filter2) });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidTIlterTypeAllCore_fileterTypesにIFilterを継承していない型が含まれる場合はfalseが返る()
    {
        var result = FilterSupport.IsValidFilterTypeAllCore(
            new Type[] { typeof(Filter1), typeof(string), typeof(Filter2) });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidTIlterTypeAllCore_fileterTypesにデフォルトコンストラクターがない型が含まれる場合はfalseが返る()
    {
        var result = FilterSupport.IsValidFilterTypeAllCore(
            new Type[] { typeof(Filter1), typeof(Filter3), typeof(Filter2) });
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidTIlterTypeAllCore_fileterTypesが全てIFilterを継承してデフォルトコンストラクターがある型の場合はtrueが返る()
    {
        var result = FilterSupport.IsValidFilterTypeAllCore(
            new Type[] { typeof(Filter1), typeof(Filter2) });
        result.Should().BeTrue();
    }

    [Fact]
    public void ThrowIfInvalidFileterTypeAllWith_fileterTypesがnullの場合は例外が発生する()
    {
        Action act = () => FilterSupport.ThrowIfInvalidFilterTypeAllWith(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ThrowIfInvalidFileterTypeAllWith_fileterTypesにnullが含まれる場合はfalseが返る()
    {
        Action act = () => FilterSupport.ThrowIfInvalidFilterTypeAllWith(
            new Type[] { typeof(Filter1), null, typeof(Filter2) });
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowIfInvalidFileterTypeAllWith_fileterTypesにIFilterを継承していない型が含まれる場合はfalseが返る()
    {
        Action act = () => FilterSupport.ThrowIfInvalidFilterTypeAllWith(
            new Type[] { typeof(Filter1), typeof(string), typeof(Filter2) });
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowIfInvalidFileterTypeAllWith_fileterTypesにデフォルトコンストラクターがない型が含まれる場合はfalseが返る()
    {
        Action act = () => FilterSupport.ThrowIfInvalidFilterTypeAllWith(
                new Type[] { typeof(Filter1), typeof(Filter3), typeof(Filter2) });
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowIfInvalidFileterTypeAllWith_fileterTypesが全てIFilterを継承してデフォルトコンストラクターがある型の場合はtrueが返る()
    {
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(
            new Type[] { typeof(Filter1), typeof(Filter2) });
    }

    public class Filter1 : IMediatorServiceFilter
    {
        public Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
        {
            throw new NotImplementedException();
        }
    }

    public class Filter2 : IMediatorServiceFilter
    {
        public Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
        {
            throw new NotImplementedException();
        }
    }

    public class Filter3 : IMediatorServiceFilter
    {
        public Filter3(string value)
        {

        }
        public Task<object> HandleAsync(RequestContext context, object parameter, Func<object, Task<object>> next)
        {
            throw new NotImplementedException();
        }
    }
}
