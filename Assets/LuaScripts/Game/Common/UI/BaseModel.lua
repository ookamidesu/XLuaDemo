
---@class BaseModel : EventSystem
---每一个model全局只有一份,为了方便使用,model都为单例,且能管理事件
BaseModel = BaseClass(EventSystem)

---@return self
function BaseModel:GetInstance()
    if self.Instance == nil then
        self.Instance = self.New()
    end
    return self.Instance
end

