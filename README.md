# Mellivora

**事先声明：该类库仅供学习和参考。[文档地址](https://nmslanx.github.io/Mellivora)**
## 历程：  
&emsp;&emsp;由于在学习中接触到了Dapper等优秀的ORM，因此激发了兴趣，想自己也尝试写一下类似的ORM。
## 技术实现
>&emsp;&emsp;本库同样是对IDbConnection进行了扩展，在起初写Mellivora的时候，动态缓存也是由纯Emit来实现的，代码量大，不容易调试。一个缓存方法写下来有小1000行。因此2017年，发布了[Natasha2016](https://github.com/dotnetcore/Natasha)版，这一版初次对IL编程进行了简化优化操作，使用Natasha对之前的Mellivora进行了缓存方法重构，代码量减少了二分之一左右，另外保证了性能。  
### 以下是两者ORM动态缓存方法的性能对比（未迁移至Core）
![Natasha缓存性能](https://nmslanx.github.io/Mellivora/images/Cache.png)
>在细节优化的时候，Mellivora参照了大量的Dapper代码，而在缓存的设计上，Mellivora更为直观。
并且配备了实体类分析库Vasily，对实体类进行分析以及Sql自动生成,从而直接支持Add、Delete、Modify、Get方法，其中Vasily中增加了对String类的[扩展](https://github.com/NMSLanX/Mellivora/blob/master/src/Vasily/Utils/NMSString.cs)，让字符串拼接更佳快速（优于StringBuilder,join）
## 性能对比  
>以下是Mellivora与Dapper预热完成之后的性能对比：
* 执行1次  
![执行1次对比](https://nmslanx.github.io/Mellivora/images/1M.png)
* 执行1000次  
![执行1000次对比](https://nmslanx.github.io/Mellivora/images/1000M.png)
* 执行10000次  
![执行10000次对比](https://nmslanx.github.io/Mellivora/images/10000M.png)
* 执行100000次  
![执行100000次对比](https://nmslanx.github.io/Mellivora/images/100000M.png)

以上库可以拿来修改，研究，发布。  
经过编写Mellivora发现，Dapper在稳定性以及性能的平衡上做的非常好，细致入微。  
开源地址：[Mellivora Github](https://github.com/NMSLanX/Mellivora)  
文档参考：[Mellivora API](https://nmslanx.github.io/Mellivora/api/index.html)

