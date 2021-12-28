---@class BaseView
BaseView = BaseClass()

---@field table
BaseView.conf = {}

function BaseView:Constructor()
    
end

function BaseView:Create()
    onLoadSucceed = function(gameObject)
        --定义一些基础属性
        self.gameObject = gameObject
        self.transform = gameObject.transform
        self.gameObject.layer = CS.UnityEngine.LayerMask.NameToLayer("UI")

        --初始化属性
        self.transform.localScale = Vector3.one
        self.transform.anchoredPosition = Vector3.zero
        local localPos = self.transform.localPosition
        localPos.z = 0
        self.transform.localPosition = localPos

        --调用初始化组件方法.子类用生成的方法进行组件绑定
        self:InitUIComponent(self.transform)

        self:OnLoad()
    end
    ResMgr:LoadPrefabGameObject(self.conf.prefabPath, onLoadSucceed)
end

function BaseView:InitUIComponent(root)
    
end

function BaseView:OnLoad()
    
end



