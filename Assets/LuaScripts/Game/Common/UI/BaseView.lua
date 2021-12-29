---@class BaseView : BaseObject
BaseView = BaseClass()

---@field table
BaseView.conf = {}


function BaseView:Constructor()
    
end

---Create 创建出对象
---@see BasePanel
---@see BaseNode
---@see UIManager
---对于 BasePanel 应该交给UIManager进行调用创建,对于 BaseNode 直接创建即可. 
---@param root table 父节点
---@param callBack function 回调
function BaseView:Create(root,callBack)
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
        callBack(self)
    end
    ObjMgr:InstantiateObjectAsync(self.conf.prefabPath, onLoadSucceed,root)
end

function BaseView:InitUIComponent(root)
    
end

function BaseView:OnLoad()
    
end



