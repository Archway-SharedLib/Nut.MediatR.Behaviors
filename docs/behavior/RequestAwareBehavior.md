# RequestAwareBehaviorの使い方

[MediatR]の[Behavior]はハンドラの前後に、バリデーションやログ出力など共通的な処理を挟み込むための優れた機構です。利用しているコンテナに登録するだけで自動的に実行されます。しかし、その構造のために次のような弱点をもっています。

- 実行順序がコンテナの実装に依存している
- 登録したBehaviorが常に全て実行される

実行順序については、登録順に実行されると[ドキュメント](https://github.com/jbogard/MediatR/wiki/Behaviors#registering-pipeline-behaviors)には記載されていますが、実際は[MediatR]側では特に制御をしていないため、コンテナの実装が将来変わった場合に、保証されなくなる可能性があります。
また、登録したBehaviorが常に全て実行されるため、リクエスト単位で実行したい/実行したくないBehaviorがある場合には制御ができません。

RequestAwareBehaviorは上記の課題に対応するために、リクエストごとに属性を利用して実行するBehaviorを指定する機能を提供します。

## 実行するBehaviorの指定

実行するBehaviorを指定するには、`WithBehaviors`属性をリクエストに設定します。`WithBehaviors`属性に利用するBehaviorのTypeを実行させたい順番で登録します。

```cs
[WithBehaviors(
    typeof(LoggingBehavior<,>),
    typeof(AuthorizationBehavior<,>),
    typeof(DataAnnotationValidationBehavior<,>))]
public class SampleRequest: IRequest<SampleResponse>
{
    [Required]
    public string Value { get; set; }
}
```

`RequestAwareBehavior`が実行時に、対象のリクエストに`WithBehaviors`属性で指定されたBehaviorを読み取り、先頭から順番に実行します。

## Behaviorの登録

`RequestAwareBehavior`は専用の`IServiceCollection`の拡張メソッドである`AddMediatRRequestAwareBehavior`を使って登録します。

```cs
new ServiceCollection()
    .AddMediatR(cfg => {
        cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    })
    .AddMediatRRequestAwareBehavior(builder =>
    {
        builder
            // 各Behaviorでアセンブリをもとに自動登録するクラスがある場合は指定する
            .AddAssembliesForAutoRegister(typeof(Program).Assembly)
            .AddLogging()
            .AddAuthorization()
            .AddDataAnnotationValidation()
            .AddOpenBehavior(typeof(FluentValidationBehavior<,>));
    })
```

`AddMediatR`側でビヘイビアが登録されている場合は、`RequestAwareBehavior`の制御下にならず、`MediatR`が直接実行するため`WithBehaviors`属性で指定した順序にならず、また`WithBehaviors`属性で指定していなくても実行されてしまうことに注意してください。

## 複数のリクエストで共通するBehavior実行の定義

多くの場合、リクエストで実行したいBehaviorは実行順序を含めて同じになります。そのため、`WithBehaviors`を継承してデフォルトのBehaviorを組み合わせた属性を定義し、その属性をリクエストに設定することを推奨します。

```cs
public class WithDefaultBehaviorsAttribute: WithBehaviorsAttribute
{
    public WithDefaultBehaviorsAttribute() : base(
        typeof(LoggingBehavior<,>),
        typeof(AuthorizationBehavior<,>),
        typeof(DataAnnotationValidationBehavior<,>))
    {
    }
}

[WithDefaultBehaviors()]
public class SampleRequest: IRequest<SampleResponse>
{
    [Required]
    public string Value { get; set; }
}
```

[MediatR]:https://github.com/jbogard/MediatR
[Behavior]:https://github.com/jbogard/MediatR/wiki/Behaviors
