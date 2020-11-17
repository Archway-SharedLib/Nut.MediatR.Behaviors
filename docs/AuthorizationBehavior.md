# AuthorizationBehaviorの使い方

AuthorizationBehaviorは認可を行うBehaviorです。リクエストごとに認可処理を実装し、対応するリクエストが実行されたときに、認可処理が実行されます。

## IAuthorizerによる認可

認可処理は`IAuthorizer<TRequest>`インターフェイスを継承したクラスを定義して`AuthorizeAsync`メソッド実装します。この実装が`AuthorizationBehavior`から呼び出されます。
戻り値には処理が成功したかどうかを指定した`AuthorizationResult`のインスタンスを返します。

```cs
public class SampleAuthorizer : IAuthorizer<SampleRequest>
{
    private readonly ICurrentUserService currentUser;

    public InsertEmployeeAuthorizer(ICurrentUserService currentUser)
    {
        this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    public Task<AuthorizationResult> AuthorizeAsync(SampleRequest request, CancellationToken cancellationToken)
    {
        if(currentUser.Roles ?? Enumerable.Empty<string>()).Contains("Admin")
        {
            return Task.FromResult(AuthorizationResult.Success());
        }
        return Task.FromResult(AuthorizationResult.Failed("Unauthorized"));
    }
}
```

失敗の結果が返された場合、`AuthorizationBehavior`は`UnauthorizedException`を発行して処理を中断します。
同じリクエストに複数の`IAuthorizer<TRequest>`が設定されている場合は、取得した順番に実行し失敗した段階で、後続する認可を実行することなく`UnauthorizedException`が発行されます。

## IAuthorizerの登録

`AuthorizationBehavior`はコンテナ経由で`IAuthorizer<TRequest>`の実装を取得するため、事前にコンテナに登録されている必要があります。