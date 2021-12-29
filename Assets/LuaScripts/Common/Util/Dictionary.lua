--自己实现的字典.

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

        --忽略以__开始的属性
        if(string.sub(key,1,2) == "__") then
            goto continue
        end
        --print("clone数据",key,value)
        if deepClone and type(value) == 'table' then
            obj[key] = value:Clone(true)
        else
            obj[key] = value
        end
        ::continue::

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










---@class Dictionary : BaseObject
Dictionary = BaseClass()

function Dictionary:Constructor(...)
    local args = ...;
    if args then
        for k, v in pairs(args) do
            self[k] = v
        end
    end

end

function Dictionary:__tostring()
    local strTable = {}
    local index = 1
    for k, v in pairs(self) do
        strTable[index] = string.format("{%s : %s}",tostring(k),tostring(v))
        index = index+1
    end
    return string.format("{%s}",table.concat(strTable,","));
end

function Dictionary:Add(key,value)
    self[key] = value
end

function Dictionary:AddRange(...)
    local args = ...;
    if args then
        for k, v in pairs(args) do
            self[k] = v
        end
    end
end

function Dictionary:Remove(key)
    local hasKey = self:ContainsKey(key)
    if hasKey then
        self[key] = nil
    end
    return hasKey
end

function Dictionary:Length()
    local count = 0
    for k, v in pairs(self) do
        count = count +1
    end
    return count
end

function Dictionary:Count(match)
    local count = 0
    for k, v in pairs(self) do
        if match(k,v) then
            count = count+1
        end
    end
    return count
end

function Dictionary:ContainsKey(key)
    return self[key] ~= nil
end

function Dictionary:Clear()
    local key;
    repeat
        key = next(self,key)
        if key ~= null then
            self[key] = nil
        end
    until key == nil
end


--暂时不写迭代器
--[[function Dictionary:__pairs()

    local obj = self
    index = nil
    return function ()
        
        index = next(obj,index)
        return index,obj[index]
    end
end]]





--[[
local key1 = {

    id=1;
    __tostring = function()
        return "123"
    end
}

local key2 = {

    id=2;
    __tostring = function()
        print(111)
        return "123"
    end
}

local dic = Dictionary.New({ [key1] = 1, [key2] = 2})

print("***********测试添加***********")
dic:Add({id=3},3)
print(dic)

print("***********测试删除***********")
dic:Remove(key1)   
print(dic)

print("***********测试含有***********")
print(dic:ContainsKey(key2)) -- true
print(dic:ContainsKey(key1)) -- false

print("***********测试数量***********")
print(dic:Length()) --2

print(dic:Count(function(k,v) 
    return v < 3
end)) --1

dic:Clear()

print(dic:Length())]]

