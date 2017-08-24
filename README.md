# Mellivora

**事先声明：该类库仅供学习和参考。[文档地址](https://nmslanx.github.io/Mellivora)**
## 历程：  
&emsp;&emsp;这个库的由来其实很不光彩，由于刚学.NET的时候老师分组做项目，我们组的项目因为后端数据没有对接上前端的控件（WPF）而落败，就像卡了壳的枪....接下来的学习里，接触了Dapper、EF等ORM，当时对于市场以及技术还处于主观任性的阶段，因此把EF、WCF、WebService、WebForm等技术拉黑，而去迎接一些轻量级的框架，例如[Dapper](https://github.com/StackExchange/Dapper). 由于本人悟性太差，无法驾驭这个高深的ORM心法, Dapper的源码如同蝌蚪文一样晦涩难懂，其源码在电脑里就像一股强劲的内力没办法化解，憋的实在难受，直到遇见了[PetaPoco](https://github.com/CollaboratingPlatypus/PetaPoco)，源码思路清晰易懂，对化解Dapper内力有着强劲的辅助作用，直到看完之后感觉有些穴道被打通了，整个人也精神了很多,反观Dapper依旧破烂不堪。  
## 技术实现
>&emsp;&emsp;在起初写Mellivora的时候，动态缓存也是由纯Emit来实现的，代码量大，不容易调试。一个缓存方法写下来有小1000行。因此2017年，发布了[Natasha2016](https://github.com/dotnetcore/Natasha)版，这一版初次对IL编程进行了简化优化操作，使用Natasha对之前的Mellivora进行了缓存方法重构，代码量减少了二分之一左右，另外保证了性能。  
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

