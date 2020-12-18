# FluentValidationBehaviorの使い方

FluentValidationBehaviorは[FluentValidation]を利用してバリデーションを行います。リクエストに紐づいている全てのバリデーターをコンテナから取得し実行します。[FluentValidation]でのバリデーションの定義方法などの詳細はリンク先を確認してください。

```cs
public class SampleValidator: AbstractValidator<SampleRequest>
{
    public SampleValidator()
    {
        RuleFor(v => v.Value).MaximumLength(20);
    }
}
```

[FluentValidation]:https://fluentvalidation.net/