
--类的声明
function BaseClass(super)
    --定义类的声明.相当于c# type
    local classType = {}
    --构造器
    classType.Constructor=false
    --如果没有指定super.默认继承自Object .Object的super为nil
    classType.super = super and super or BaseObject and BaseObject or nil
    classType.New = function(...)
        local obj = {}
        local create
        
        create = function(c, obj, ...)
            if c.super then
                create(c.super, obj, ...)   --递归调用所有父类的构造方法
            end
            if c.Constructor then
                c.Constructor(obj,...)
            end
        end
        --设置元表
        setmetatable(obj,classType)
        --因为在声明过程中.classType设置__index为自己.所有obj找不到数据依然会往classType中寻找
        
        --print("new",obj.__classType)
        create(classType, obj, ...)
        --设置Object的classType
        
        return obj
    end

    --如果super存在.代表不是Object.设置classType的元表
    --print("classType",classType)
    --print(classType.super)
    if classType.super then
        setmetatable(classType,classType.super)
    end
    --将classType的__index设置为自己.以供字表使用
    classType.__index = classType
    classType.__classType = classType
    return classType
end

print("**************************")
---@class BaseObject
BaseObject = BaseClass()
function BaseObject:Clone(deepClone)
    --print("开始Clone",self.__classType)
    local obj = self.__classType.New();
    --print(self.id)
    
    for key, value in pairs(self) do
        
        --print("clone数据",key,value)
        if deepClone and type(value) == 'table' then
            obj[key] = value:Clone(true) 
        else
            obj[key] = value
        end
    end
    return obj;
end

function BaseObject:Destroy()
    if self.is_destroyed then  --是否已经调过一次DeleteMe
        return
    end
    self.is_destroyed = true

    local nowSuper = self.__classType
    while nowSuper ~= nil do
        local onDestroy = rawget(nowSuper, "OnDestroy")
        if onDestroy then  --每一个类调OnDestroy方法,需要对象自行将数据置为nil
            onDestroy(self)
        end
        nowSuper = nowSuper.super
    end
end

function BaseObject:OnDestroy()
    --print("基础的销毁方法")
end

function BaseObject:__tostring()
    local strTable = {}
    local count = 1
    for key, value in pairs(self) do
        if type(value) ~= "function" and string.sub(key,1,2) ~= "__" then
            strTable[count] = string.format("%s:%s",tostring(key),tostring(value));
            count = count+1
        end
    end
    
    return string.format("[%s]",table.concat(strTable,","));
end
    
    

BaseObject.__index = BaseObject

--[[--测试继承自Object的基础方法
TestClass = BaseClass()
TestClass.id = 0

local obj = Object.New()
obj.id = 1;

function  TestClass:OnDestroy()
    --print("被销毁")
end

function  TestClass:Constructor(data)
    print("创建",data)
    print(self)
end




]]--[[function TestClass:ToString()
    return "321"
end]]--[[
local o1 = TestClass.New(1)
local o2 = TestClass.New(2)
print(o2.id)

TestClass1 = BaseClass(TestClass)
function  TestClass1:Constructor(data)
    print("创建test1",data)
    print(self)
end
local ba = TestClass1.New(10)]]

--[[print(o1.__classType == o2.__classType)
o2.id = 2;
o2.obj = obj

o3 = o2:Clone(true);
o2.id = 3

print(o2)
print(o3)


print("*********原本数据************")
for i, v in pairs(o2) do
   print(i,v)
end

print("*********Clone后的数据************")
for i, v in pairs(o3) do
    print(i,v)
end

print(o2.obj.id == o3.obj.id)
print(o2.obj == o3.obj)

o2:Destroy()
o3:Destroy()]]

--一下代码测试oop
--[[
print("*********************Object id",Object)

local obj1 = Object.New()

print("************封装*************")

--使用id.当obj中找不到.会玩元表中找.
print(obj1.id)
--设置id.
obj1.id = 1
--因为上一步已经什声明了id.这次直接能找到
print(obj1.id)


local obj2 = Object.New()
--因为obj1更改id为自己的.所以不影响obj1的id
print(obj2.id)

print("************继承*************")

--声明一个Person类.继承Object
Person = BaseClass()

--定义Person的属性
Person.name = "人"
Person.age = 0
function Person:Say()
    print(string.format("我叫%s,今年%s",self.name,self.age))

end

--创建一个Person,自己没有定义new.因为通过subClass定义类,会找元表Object中的new方法
local p1 = Person.New()

--调用Person的方法
p1:Say()

--声明一个Student类.继承Person
Student = BaseClass(Person)
--定义Student的属性
Student.grade = ""
function Student:GoSchool()
    print(string.format("%s去上学了",self.name))
end

--创建一个Student,自己没有定义new.因为通过subClass定义类,会找元表Student中的new方法.而Student会找Object的new
local s1 = Student.New()
--Student没有定义name.会找元表Person中的定义
print(s1.name)
--设置自己的name
s1.name = 'gg'
s1:Say()
s1:GoSchool()

print("************多态*************")
--因为lua定义变量不使用类型声明.无法使子类引用指向父类.这里直接演示继承,重写...



--重写父类的Say方法.因为自己没有定义Say,所以这里本质是自己生命一个Say方法
function Student:Say()
    print(string.format("我叫%s,今年%s.现在是一名学生",self.name,self.age))
end
s1:Say()

--重写父类的Say方法.且调用父类的方法.使用base.Say并将self传入进去.不能使用self.base:Say().这样会将self.base传入进去.无法在父类中使用子类的对象
function Student:Say()

    self.super.Say(self)
end

s1:Say()
]]

