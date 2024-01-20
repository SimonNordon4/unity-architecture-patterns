In this installment we'll be taking a look at the Scriptable Object Component.

Although the name of this pattern can be a little deceiving.

Unity Engine offers Scriptable Objects by default, and in fact, we used hundreds of them 
in the Spaghetti and GameObject-Component pattern. [Image of Scriptable Object]

So what exactly is the Scriptable Object pattern?

The typical use case for Scriptable Objects is to serialize large amounts of unchanging data.
In this way, they're kind of like a configuration file, or definition, and indeed this how they where used in the previous
two patterns.

The Scriptable Object Pattern asks, what if we pushed Scriptable Objects to their limits?

Not only can we use them as definitions, but we can also use them for events, runtime data, and even entirely replace some components.

## Why Use Scriptable Objects.

When we explored the GameObject Component Pattern [link] we saw that it was a great way to organize our code, and make it more modular.

However one of the major draw backs was the difficulty in resolving scene dependencies, which ultimately made one question whether or not it was
worth using at all. 

The Scriptable Object Pattern promises to remove this problem, but not having scene dependcies to begin with.

It even offers to be a replacement for the Singleton Pattern, keeping all the benefits while eliminating the downsides.

## Creating a new framework.

Unity's defualt scriptable object is a great starting point, however on their own they're not good enough for a full Scripable Object Framework.

In order to fully leverage their power, we're going to need to get our hands dirty and create a custom framework.

### Scriptable Data

One draw back of Scriptable Objects is that changes during run time are permenenant.
This will be a problem if we want to use them for runtime data which needs to reset between every session.

Solving this problem requires some verbose cose, but we can hide this complexity behind the first base class of our new Framework.

The ScriptableData class.

```csharp
public abstract ScriptableData : ScriptableObject
{
    public abstract void Reset();
}
```

### Scriptable Persistent Data

While we're at it, we may as well introduce an extension of this class that can save and load it's own data.

```csharp
public abstract ScriptablePersistentData : ScriptableData
{
    public abstract void Save();
    public abstract void Load();
}
```

### Scriptable Event

The next base class we'll need is for Scriptable Events.

```csharp
public abstract ScriptableEvent : ScriptableObject
{
    public abstract void Raise();
}
```

### Scriptable Singleton

Ironically, there are some cases where a Singleton is actually desirable. 
For example if we wanted to create a boot strapper for our project, we'd want to be sure there was only one.

A scriptable Singleton adds some challenges as not only do we only want a single instance, we only want a single copy in the project as well.

```csharp
public abstract ScriptableSingleton : ScriptableObject
{
    public abstract void Initialize();
}
```

### Scriptable Factory and Scriptable Pool

Scriptable Objects are Unity Objects under the hood, which means they can Instantiate GameObjects. 
We can leverage this to create a Object Pools that aren't scene dependent.

```csharp
public abstract ScriptableFactory : ScriptableObject
{
    public abstract GameObject Create();
}
```



## Making it feel like a framework.

The DNA of the framework is actually fairly straight forward and easy to replicate. But I wanted it to really feel like it's own Framework. 
Something that Unity themselves could (should?) have made.

That means adding all the Editor bells and whistles that we've come to expect from Unity, to make
this framework feel like it's part of the engine, and just fun to use.

If you want to try out the Scriptable Object Framework for yourself, you can download it from the Unity Asset Store. [Link]

Otherwise you can use the Free One we're exploring in this framework, it has all the same functionalities without the quality of life features.

### Replacing Singletons without the extra work.

Here's a must know tip. If you want to replace a Singleton with a Scriptable Object, you can do so without changing any code.

Set the Scriptable Object as the default reference in your mono-script. Every new instance of the mono-script will now have the Scriptable Object automatically assigned.

If you're using the Scriptable Object Extensions Pack [link] you can even update this at any time.

### Making up for the Sins of GameObject Component.

If we recall from the GameObject Component architecture, the code was clean and it did deliver on it's promise of making code more modular.
However the issues with scene dependencies made it difficult to use in practice.

It was obvious what need to be replaced first, all scene dependent data. At a glance that included the following:

- Game State Events
- Level
- Player Data such as Current Health, Position etc.
- Round Spawner Events
- Store
- Gold
- Achievements
- Statistics
- UI State Events
- Settings

These straight forward changes instantly reduced the complexity of GameObject Component by 50%, while keeping the code clean.

