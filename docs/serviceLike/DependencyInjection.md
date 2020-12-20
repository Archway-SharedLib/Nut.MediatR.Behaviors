# ServiceLike.DependencyInjection

Nut.MediatR.ServiceLike.DependencyInjectionは[Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/ja-jp/dotnet/core/extensions/dependency-injection)の`IServiceCollection`を通して、Nut.MediatR.ServiceLikeを設定します。

```cs
services
    .AddMediatR(typeof(Startup))
    .AddMediatRServiceLike(typeof(Startup).Assembly, typeof(Filter1), typeof(Filter2));
```

引数で指定されたアセンブリの中から`AsServiceAttribute`属性が付与された`IRequest`の実装を探索し、`RequestRegistry`に引数で指定されているフィルターの`Type`とともに登録します。

`DefualtMediatorClient`は`IMediatorClient`型で登録されるため、利用する場合は`IMediatorClient`をインジェクションします。

```cs
public class SampleService 
{
    private readonly IMediatorClient client;

    public SampleService(IMediatorClient client)
    {
        this.client = client;
    }

    public async Task<Output> Do() 
    {
        return await client.SendAsync(new { Id = "123"}).ConfigureAwait(false);
    }
}
```

## RequestRegisterの登録

DIコンテナには`RequestRegister`がSingletonで登録されます。すでに登録されている場合は、そのインスタンスを取り出して利用します。
つまり、`AddMediatRServiceLike`を何度呼び出しても、`RequestRegister`のインスタンスは同じものが利用されます。