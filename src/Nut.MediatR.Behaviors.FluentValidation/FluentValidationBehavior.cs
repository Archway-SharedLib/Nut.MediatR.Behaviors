using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR;

/// <summary>
/// <see cref="FluentValidation"/> で指定されたバリデーションを実行します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    public FluentValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// 定義された <see cref="IValidator{T}"/> を取得してバリデーションを実行します。
    /// </summary>
    /// <param name="request">リクエストの値</param>
    /// <param name="next">次の処理</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>処理の結果</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validators = _serviceProvider.GetServicesOrEmpty<IValidator<TRequest>>();
        if (validators?.Any() == true)
        {
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken))).ConfigureAwait(false);
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
            if (failures.Any()) throw new ValidationException(failures);
        }
        return await next().ConfigureAwait(false);
    }
}

