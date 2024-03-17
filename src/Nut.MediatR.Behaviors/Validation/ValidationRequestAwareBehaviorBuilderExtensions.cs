namespace Nut.MediatR;

/// <summary>
/// <see cref="RequestAwareBehaviorBuilder"/> の拡張メソッドを定義します。
/// </summary>
public static class ValidationRequestAwareBehaviorBuilderExtensions
{
    /// <summary>
    /// データアノテーションによるバリデーションを行う <see cref="DataAnnotationValidationBehavior{TRequest, TResponse}"/> を追加します。
    /// </summary>
    /// <param name="builder"><see cref="RequestAwareBehaviorBuilder"/></param>
    /// <returns>元となった <see cref="RequestAwareBehaviorBuilder"/></returns>
    public static RequestAwareBehaviorBuilder AddDataAnnotationValidation(this RequestAwareBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(DataAnnotationValidationBehavior<,>));
        return builder;
    }
}
