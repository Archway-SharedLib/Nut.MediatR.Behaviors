# DataAnnotationValidationBehaviorの使い方

DataAnnotationValidationBehaviorはリクエストに設定された[データ属性](https://docs.microsoft.com/ja-jp/dotnet/api/system.componentmodel.dataannotations)を利用したバリデーションを行います。

```cs
public class SampleRequest: IRequest<SampleResponse>
{
    [Required]
    public string Value { get; set; }
}
```

リクエストにデータ属性を指定しておくだけで自動的に実行されます。