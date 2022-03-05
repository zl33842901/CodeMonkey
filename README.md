# CodeMonkey
使用 SourceGenerator 生产代理类实现切片编程（AOP）的包。

### 特点：

基于 SourceGenerator ，比动态代理启动更快，运行也更快；但功能没有动态代理强大。 适用于服务类特别多，动态代理性能产生问题的项目。


### 安装程序包：

dotnet add package xLiAd.CodeMonkey.Generator

### 使用方法：

1，在要使用 AOP 的项目里引用程序包，并定义一个 Attribute，名称无所谓，但是 格式要求： 需要有一个 public string BeforeBody 或 public string AfterBody。BeforeBody 定义方法或属性执行前的语句，AfterBody 定义方法或属性执行后的语句。

2，在需要使用 AOP 的类上（或其中的方法上）使用这个 Attribute，然后生成你的项目。

3，反射查找你生成的代理类型，通常如果有代理类型 就使用代理类型，如果没有代理类型就使用原始类型。参见两个 Sample。

### 注意：
由于 SourceGenerator 本身的限制，Attribute 需要和 你的服务类在同一个项目中；
另：服务类可见性现在要求是 public。