Pool and reuse collections and other object types to minimize memory allocation/garbage collection.

## Install

From your unity project folder:

    npm init
    npm install beatthat/pools --save

The package and all its dependencies will be installed under Assets/Plugins/packages/beatthat.

In case it helps, a quick video of the above: https://youtu.be/Uss_yOiLNw8

## USAGE

#### Use a List in a ```using``` block

The list type returned is a Disposable whose dispose method returns the list to the pool so...

```csharp
using(var list = ListPool<string>.Get() {
  // use the list
}
```

#### Use a pooled list and return it manually when you're done

Usually you should wrap the list's use in a using block as above, but you can't do that if, say, you're going to do something asynchronous with the list.

```csharp
var list = ListPool<string>.Get();
SomeAsyncMethod(list, () => {
  // have to dispose list manually
  // because its use extends 
  // into async callback
  
  list.Dispose()
})
```

#### Use a pooled copy of some other collection

If you're using a pooled list to work with a copy of some other collection already in hand, you can pass the collection you want to copy into ```Get``` and the returned list will already have the contents copied for you.

```csharp
IDictionary<string, string> d = SomeDictionary();
using(var list = ListPool<string>.Get(d.Values) {
  // list has all the values from d
}
```

#### Pools for Dictionary and StringBuilder work similarly to list

```csharp
IDictionary<string, string> d = SomeDictionary();
using(var pooledDict = DictionaryPool<string, string>.Get(d) {
  // list has all the values from d
}

using(var sb = PooledStringBuilder.Get() {
    // sb.stringBuilder holds the StringBuilder 
    // in an IDisposable wrapper
}
```

#### ArrayPool requires a size or a collection/array to copy from

```csharp
using(var a = ArrayPool<string>.Get(3) {
  // array will have 3 elements

  // actual array is in a.array
  // because array itself can't be extended to implement IDisposable
}

IDictionary<string, string> d = SomeDictionary();
using(var a = ArrayPool<string>.Get(d.Values) {
  // array will have all the values of d as elements

  // actual array is in a.array
  // because array itself can't be extended to implement IDisposable
}

```

#### Use StaticObjectPool<T> to pool other types

You can use StaticObjectPool<T> to pool other c# types as long as they have a no-arg constructor

```csharp
class Foo {}

class Bar {
  void SomeMethod()
  {
    var foo = StaticObjectPool<Foo>.Get();
    // do stuff with Foo
    StaticObjectPool<Foo>.Return(foo);
  }
}
```
