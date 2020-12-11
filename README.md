<img src="./assets/logo/logo.svg" alt="logo" height="192px" style="margin-bottom:2rem;" />

[![CI](https://github.com/Archway-SharedLib/Nut.MediatR.Behaviors/workflows/CI/badge.svg)](https://github.com/Archway-SharedLib/Nut.MediatR.Behaviors/actions)
[![codecov](https://codecov.io/gh/Archway-SharedLib/Nut.MediatR/branch/main/graph/badge.svg?token=N7arqXbSNk)](https://codecov.io/gh/Archway-SharedLib/Nut.MediatR)

Nut.MediatRは[MediatR]を利用した、様々な機能を提供します。

## Nut.MediatR.Behaviors

Nut.MediatR.Behaviorsは[MediatR]の、様々なアプリケーションで利用できる汎用のカスタム[Behavior]を提供します。次のBehaviorが含まれます。

- [PerRequestBehavior](./docs/PerRequestBehavior.md)
- [AuthorizationBehavior](./docs/AuthorizationBehavior.md)
- [LoggingBehavior](./docs/LoggingBehavior.md)
- [DataAnnotationValidationBehavior](./docs/DataAnnotationValidationBehavior.md)

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

Nut.MediatR.Behaviors.FluentValidationは[FluentValidation]を利用したバリデーションを実行する、[MediatR]の[Behavior](https://github.com/jbogard/MediatR/wiki/Behaviors)を提供します。

詳細は[ドキュメント](./docs/FluentValidationBehavior.md)を参照してください。

[MediatR]:https://github.com/jbogard/MediatR
[Behavior]:https://github.com/jbogard/MediatR/wiki/Behaviors
[FluentValidation]:https://fluentvalidation.net/

## Nut.MediatR.ServiceLike

> This library is not yet available.

Nut.MediatR.ServiceLikeはMediatRのハンドラを、まるでサービスのように文字列のパスを指定して実行できるようにするライブラリです。
Nut.MediatR.ServiceLikeを利用することで、`IRequest`の実装自体への依存も無くせます。

```cs
[AsService("/users/detail")]
public record UserQuery(string UserId): IRequest<UserDetail>;

public class UserService 
{
    private readonly IMediatorClient client;

    public GetUserService(IMediatorClient client)
    {
        this.client = client;
    }

    public async Task<User> Get(string id)
    {
        var result = await client.Send<User>("/users/detail", new {UserId = id});
        return result;
    }
}
```