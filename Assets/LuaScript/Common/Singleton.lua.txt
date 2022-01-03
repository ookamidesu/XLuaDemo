
---@class Singleton
Singleton = BaseClass()

Singleton.Instance = nil


---@return self
function Singleton:GetInstance()
    if self.Instance == nil then
        self.Instance = self.New()
    end
    return self.Instance
end



--[[
print("*********测试单例*************")
TestSingleton = BaseClass(Singleton)
TestSingleton1 = BaseClass(Singleton)

function TestSingleton:Constructor()
    self.id = 1
end

function TestSingleton1:Constructor()
    self.data = 2
end

print(TestSingleton:GetInstance().id)
TestSingleton:GetInstance().id = 2
print(TestSingleton:GetInstance().id)
print(TestSingleton1:GetInstance().data)]]
