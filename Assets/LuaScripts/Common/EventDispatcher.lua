---@class EventSystem : BaseObject
---事件派发器.
local EventDispatcher = BaseClass()

function EventDispatcher:Constructor()
    ---@type Dictionary | List[] | function[][]
    self.allEvents = Dictionary.New()
    --self.allEvents[1].
end