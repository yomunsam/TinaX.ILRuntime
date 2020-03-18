# TinaX Framework - ILruntime.

[![LICENSE](https://img.shields.io/badge/license-NPL%20(The%20996%20Prohibited%20License)-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
[![LICENSE](https://camo.githubusercontent.com/3867ce531c10be1c59fae9642d8feca417d39b58/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f636f6f6b6965592f596561726e696e672e737667)](https://github.com/yomunsam/TinaX/blob/master/LICENSE)

`VFS.ILRuntime` 为 Unity 开发框架 `TinaX` 提供了基于`ILRuntime`实现的C#热更新能力.

`VFS.ILRuntime` provides C# hotfix capability based on` ILRuntime` for Unity development framework `TinaX`

<br>

您可以使用Unity Package Manager来安装使用该包。

You can use the Unity Package Manager to install and use this package.  

```
git://github.com/yomunsam/TinaX.ILRuntime.git
```

package name: `io.nekonya.tinax.ilruntime`

<br>
------

## Dependencies

在安装之前，请先确保已安装如下依赖：

Before setup `TinaX.VFS`, please ensure the following dependencies are installed by `Unity Package Manager`:

- [io.nekonya.tinax.vfs](https://github.com/yomunsam/UniRx.UPM) :`git://github.com/yomunsam/TinaX.VFS.git`
- [io.nekonya.tinax.core](https://github.com/yomunsam/tinax.core) :`git://github.com/yomunsam/TinaX.Core.git`

------

## Third-Party

本项目中使用了以下优秀的第三方库：

The following excellent third-party libraries are used in this project:

- **[ILRuntime](https://github.com/Ourpalm/ILRuntime)** : Pure C# IL Intepreter Runtime, which is fast and reliable for scripting requirement on enviorments, where jitting isn't possible.