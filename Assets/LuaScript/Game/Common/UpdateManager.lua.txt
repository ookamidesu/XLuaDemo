--帧循环

---@class UpdateManager : Singleton
---@field allObservers List
UpdateManager = BaseClass(Singleton)


function UpdateManager:Constructor()
    --定义成员变量
    
    ---@type List
    self.allObservers = List.New()
end

function UpdateManager:Register(observer)
    self.allObservers.Add(observer)
end

function UpdateManager:Unregister(observer)
    self.allObservers.Remove(observer)
end


function UpdateManager:Update(deltaTime, unscaledDeltaTime)
    for observer in self.allObservers:GetIterator() do
        if observer.Update then
            observer.Update(deltaTime, unscaledDeltaTime)
        end
    end
end

function UpdateManager:LateUpdate()
    for observer in self.allObservers:GetIterator() do
        if observer.LateUpdate then
            observer.LateUpdate(deltaTime, unscaledDeltaTime)
        end
    end
end

function UpdateManager:FixedUpdate(fixedDeltaTime)
    for observer in self.allObservers:GetIterator() do
        if observer.FixedUpdate then
            observer.FixedUpdate(fixedDeltaTime)
        end
    end
end

function Update(deltaTime, unscaledDeltaTime)
    --Time:SetDeltaTime(deltaTime, unscaledDeltaTime)

    UpdateManager:GetInstance():Update(deltaTime,unscaledDeltaTime)
end

function LateUpdate()
--[[    LateUpdateBeat()
    CoLateUpdateBeat()
    Time:SetFrameCount()]]
    UpdateManager:GetInstance():LateUpdate()
end

function FixedUpdate(fixedDeltaTime)
--[[    Time:SetFixedDelta(fixedDeltaTime)
    FixedUpdateBeat()
    CoFixedUpdateBeat()]]
    UpdateManager:GetInstance():FixedUpdate(fixedDeltaTime)
end

--[[
--测试
TestUpdate = BaseClass(Singleton)

function TestUpdate:Constructor()
    UpdateManager:GetInstance():Register(self)
end

function UpdateManager:Update(deltaTime, unscaledDeltaTime)
    print("Update",deltaTime,unscaledDeltaTime)
end

function UpdateManager:LateUpdate()
    print("LateUpdate")
end

function UpdateManager:FixedUpdate(fixedDeltaTime)
    print("FixedUpdate",fixedDeltaTime)
end]]
