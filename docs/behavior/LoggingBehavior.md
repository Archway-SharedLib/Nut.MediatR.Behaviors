# LoggingBehaviorの使い方

LoggingBehaviorはログの出力を行うBehaviorです。
リクエストの実行前後でログを出力します。また`ILoggingInOutValueCollector`をリクエストごとに実装することで、引数と戻り値をログに含めることもできます。

ログ出力の実装は[Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/)を利用します。

## 出力されるログの内容

リクエストが実行される前のログは次の形式でメッセージが出力されます。

```
Start {Request}. {Input}
```

`{Request}`にはリクエストの型の名前が設定されます。
リクエストに対応する`ILoggingInOutValueCollector`が実装されている場合は、`{Input}`に出力されます。

リクエストが実行された後のログは次の形式でメッセージが出力されます。

```
Complete {Request} in {Elapsed}ms. {Output}
```

`{Request}`にはリクエストの型の名前が設定されます。また{Elapsed}には実行時間がミリ秒で設定されます。
リクエストに対応する`ILoggingInOutValueCollector`が実装されている場合は、`{Output}`に出力されます。

リクエストの実行中に例外が発生した場合は、次の形式のメッセージでログが出力されます。

```
Exception {Request} in {Elapsed}ms.
```

`{Request}`にはリクエストの型の名前が設定されます。また{Elapsed}には実行時間がミリ秒で設定されます。

## ILoggingInOutValueCollectorによる引数/戻り値の出力

ログに引数および戻り値を出力する場合は、`ILoggingInOutValueCollector<TRequest, TResponse>`を継承した実装を追加します。

出力する引数を指定するには、`CollectInValueAsync`メソッドを実装します。
出力する戻り値を指定するには、`CollectOutValueAsync`メソッドを実装します。

```cs
public class SampleLoggingInOutValueCollector : ILoggingInOutValueCollector<SampleRequest, SampleResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(request.Value));
    }

    public Task<InOutValueResult> CollectOutValueAsync(SampleResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(response.Value));
    }
}
```

両方のメソッドともに戻り値は`InOutValueResult`の`WithValue`メソッドを利用して作成します。`InOutValueResult`には`Empty`メソッドもありますが、これを利用して作成した戻り値は、`LoggingBehavior`側では、出力する値が指定されていないのと同義として扱われるため、値は出力されません。

片方の値、例えば引数だけ出力したい場合などは`BaseLoggingInOutValueCollector<TRequest, TResponse>`を利用してください。`BaseLoggingInOutValueCollector<TRequest, TResponse>`は引数と戻り値ともに`InOutValueResult`の`Empty`が返る基底の実装がされているため、値を返したい方の実装を行うだけで実現できます。

```cs
public class SampleLoggingInOutValueCollector : BaseLoggingInOutValueCollector<SampleRequest, SampleResponse>
{
    public override Task<InOutValueResult> CollectInValueAsync(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(request.Value));
    }
}
```

## ILoggingInOutValueCollectorの登録

`LoggingBehavior`はコンテナ経由で`ILoggingInOutValueCollector<TRequest, TResponse>`の実装を取得するため、事前にコンテナに登録されている必要があります。

## デフォルトのILoggingOutValueCollectorの設定

`LoggingBehavior`を継承して、`GetDefaultCollector`メソッドをオーバーライドすることでデフォルトの`ILoggingInOutValueCollector<TRequest, TResponse>`を設定できます。
`LoggingBehavior`はコンテナ経由で`ILoggingInOutValueCollector<TRequest, TResponse>`が取得できなかった場合に、この`GetDefaultCollector`メソッドの結果を利用します。

```cs
// デフォルトで利用する ILoggingInOutValueCollector<TRequest, TResponse> の実装
public class ToStringLoggingInOutValueCollector<TRequest, TResponse> :
    ILoggingInOutValueCollector<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(request.ToString()));
    }

    public Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.WithValue(response.ToString()));
    }
}

// デフォルトで ILoggingInOutValueCollector<TRequest, TResponse> 利用する LoggingBehavior<TRequest, TResponse> の実装
public class CustomLoggingBehavior<TRequest, TResponse> :
    LoggingBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{

    public CustomLoggingBehavior(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    protected override ILoggingInOutValueCollector<TRequest, TResponse> GetDefaultCollector()
    {
        return new ToStringLoggingInOutValueCollector<TRequest, TResponse>();
    }
}

```
