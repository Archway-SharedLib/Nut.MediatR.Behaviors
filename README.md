<img src="./assets/logo/logo.svg" alt="logo" height="192px" style="margin-bottom:2rem;" />

[![CI](https://github.com/Archway-SharedLib/Nut.MediatR.Behaviors/workflows/CI/badge.svg)](https://github.com/Archway-SharedLib/Nut.MediatR.Behaviors/actions)

Nut.MediatRは[MediatR]を利用した、様々な機能を提供します。

## Nut.MediatR.Behaviors

[![NuGet](https://img.shields.io/nuget/vpre/Nut.MediatR.Behaviors.svg)](https://www.nuget.org/packages/Nut.MediatR.Behaviors) 
[![NuGet](https://img.shields.io/nuget/dt/Nut.MediatR.Behaviors.svg)](https://www.nuget.org/packages/Nut.MediatR.Behaviors)

Nut.MediatR.Behaviorsは[MediatR]の、様々なアプリケーションで利用できる汎用のカスタム[Behavior]を提供します。次のBehaviorが含まれます。

- [PerRequestBehavior](./docs/behavior/PerRequestBehavior.md)
- [AuthorizationBehavior](./docs/behavior/AuthorizationBehavior.md)
- [LoggingBehavior](./docs/behavior/LoggingBehavior.md)
- [DataAnnotationValidationBehavior](./docs/behavior/DataAnnotationValidationBehavior.md)

```cs
[WithBehaviors(
    typeof(LoggingBehavior),
    typeof(AuthorizationBehavior),
    typeof(DataAnnotationValidationBehavior)
)]
public class ProductQuery: IRequest<ProductQueryResult> {
}
```

詳細は各リンク先を参照してください。

## Nut.MediatR.Behaviors.FluentValidation

[![NuGet](https://img.shields.io/nuget/vpre/Nut.MediatR.Behaviors.FluentValidation.svg)](https://www.nuget.org/packages/Nut.MediatR.Behaviors.FluentValidation) 
[![NuGet](https://img.shields.io/nuget/dt/Nut.MediatR.Behaviors.FluentValidation.svg)](https://www.nuget.org/packages/Nut.MediatR.Behaviors.FluentValidation)

Nut.MediatR.Behaviors.FluentValidationは[FluentValidation]を利用したバリデーションを実行する、[MediatR]の[Behavior](https://github.com/jbogard/MediatR/wiki/Behaviors)を提供します。

詳細は[ドキュメント](./docs/behavior/FluentValidationBehavior.md)を参照してください。

[MediatR]:https://github.com/jbogard/MediatR
[Behavior]:https://github.com/jbogard/MediatR/wiki/Behaviors
[FluentValidation]:https://fluentvalidation.net/
