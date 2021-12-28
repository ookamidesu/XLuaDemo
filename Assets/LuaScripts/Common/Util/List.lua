--自己实现的list.下标从1开始

---@class List : BaseObject
List = BaseClass()
List.data = {}
List.id = 0


function List:Constructor(...)
    local datas = {}
    self.data = datas
    local args = {...};
    for i = 1, #args do
        self.data[i] = args[i]
    end
    self.id = 0
    
    setmetatable(self,datas)
    datas.__index = datas
    datas.__tostring = List.__tostring
    datas.__newindex  = datas
    setmetatable(datas,self.__classType)
end

--传入元素,获取索引
function List:IndexOf(item)
    for i = 1, #self.data do
        if self[i] == item then
            return i
        end
    end
    return -1
end

--传入委托,获取索引
function List:ElementAt(match)
    for i = 1, #self.data do
        if match(self.data[i])then
            return i
        end
    end
    return -1
end

function List:FindFirst(match)
    for i = 1, #self.data do
        if match(self.data[i])then
            return self.data[i]
        end
    end
    return nil
end

function List:FindLast(match)
    for i = #self.data,1 ,-1  do
        if match(self.data[i])then
            return i
        end
    end
    return nil
end

function List:Add(item)
    table.insert(self.data,item)
end

function List:AddRange(...)
    local args = {...};
    for _, v in pairs(args) do
        self:Add(v)
    end
end

function List:Insert(index,item)
    --print(index,item)
    table.insert(self.data,index,item)
end

function List:InsertRange(index,...)
    local args = {...};
    --print(index)
    for i = #args,1 ,-1 do
        self:Insert(index,args[i])
    end
end

function List:RemoveAt(index)
    return table.remove(self.data,index)
end

function List:Remove(item)
    local index = self:IndexOf(item)
    if index <= 0 then
        return false
    end
    self:RemoveAt(index)
    return true
end

function List:RemoveAll(item)
    local flag = false
    while self:Remove(item) do
        flag = true
    end
    return flag
end


function List:Length()
   return #self.data
end

--计算数量,传入委托
function List:Count(match)
    if match then
        local count = 0
        for i = 1, #self.data do
            if match(self.data[i]) then
                count = count+1
            end
        end
       
        return count
    end
    return self:Length()
end

function List:Sort(comparer)
   table.sort(self.data,comparer)
end

--迭代器
---@type fun()
function List:GetIterator()
    local index = 0
    local count = #self.data

    return function ()
        index = index + 1

        if index <= count
        then
            return self.data[index]
        end

    end
end

--[[--正序排序
--keySelector 选择器
--comparer 比较器
function List:OrderBy(keySelector,comparer)
    
end

--逆序排序
--keySelector 选择器
--comparer 比较器
function List:OrderByDescending(keySelector,comparer)

end]]



function List:__tostring() 
    local strTable = {}
    for i = 1, #self.data do
        --print(tostring(self.data[i]))
        strTable[i] = tostring(self.data[i])
    end
    return string.format("[%s]",table.concat(strTable,","));
    
end

function List:Clear()
    for i = 1, #self.data do
        self.data[i] = nil
    end
end

function List:Clone(deepClone)
    local obj = List.New()
    for i = 1, #self.data do
        --print("clone",self.data[i])
        if deepClone and type(self.data[i]) == 'table' then
            obj[i] = self.data[i]:Clone(true)
        else
            obj[i] = self.data[i]
        end
    end
    return obj
end

--测试

--[[print("**************添加相关方法****************")

local list = List.New(1,2,3,4,5)
print(list)
--[1,2,3,4,5]
list:Add(6)
print(list)
--[1,2,3,4,5,6]
list:AddRange(3,4)
print(list)
--[1,2,3,4,5,6,3,4]
list:Insert(2,5)
print(list)
--[1,5,2,3,4,5,6,3,4]
list:InsertRange(4,3,2,1)
print(list)
--[1,5,2,3,2,1,3,4,5,6,3,4]

print("**************删除相关方法****************")
print(list:Remove(1)) --true
print(list:Remove(10)) --删除一个不存在的数返回false
print(list)
--[5,2,3,2,1,3,4,5,6,3,4]
print(list:RemoveAt(2)) --2 删除索引为2的数,并返回
print(list)
--[5,3,2,1,3,4,5,6,3,4]
print(list:RemoveAll(3)) --true
print(list)
--[5,2,1,4,5,6,4]


print("**************修改****************")
--直接使 = 用[index]进行访问和修改.会直接访问和修改List.data里的数据
print(list[2]) --2
list[2] = 3
print(list)
--[5,3,1,4,5,6,4]

print("**************查询****************")
print(list:FindFirst(function(data) return data > 4 end)) --5 找到第一个大于4的数
print(list:FindLast(function(data) return data > 4 end)) --6 找到最后一个大于4的数
print(list:FindLast(function(data) return data > 10 end)) --nil

print("**************查询索引****************")
print(list:IndexOf(3)) --2 找到第一个3的索引
print(list:ElementAt(function(data) return data < 4 end)) --2 找到第一个小于4的索引

print("**************数量相关方法****************")
print(list:Length())    --7 所有元素个数
print(list:Count(function(data) return data > 3 end))   --所有大于3的个数


print("**************排序相关方法****************")
list:Sort()
print(list)
--[1,3,4,4,5,5,6]
list:Sort(function(a,b) return a>b end)
print(list)
--[6,5,5,4,4,3,1]

print("**************迭代器遍历****************")

for data in list:GetIterator() do
    print(data)
end]]

--[[local o = BaseObject.New();
o.id = 1
local datas = List.New(1,o,3,4,5)
datas.id = 2
--print(table.concat(List.data,','))
print(datas)
--print(datas[1])

local data1 =datas:Clone(true)
data1.id = 4
data1[1] = 2;
datas[2].id = 2
print(datas[2].id)
print(data1[2].id)

print(datas)
print(data1)]]

--[[
print(datas)
datas:Add(nil)
print(datas)
datas:AddRange(7,8)
print(datas)
print("**************数量相关方法****************")
print(datas:Length())
print(datas:Count(function(data) return data > 5 end))

print("**************获取相关方法****************")
print(datas:IndexOf(2))
print(datas:ElementAt(function(data) return data > 5 end))
]]


