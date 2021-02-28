
pub type WindowMsgHandler = fn (&str) -> bool;

pub type ExternalWindowUpdateFn = fn();
pub type ExternalWindowReceiveMsgFn = fn(*const u8);
pub type ExternalWindowDeliverMsgFn = fn(*const u8);

pub trait Window
{
    fn update(&mut self);

    fn send_msg(&mut self, msg: &str);

    fn register_msg_handler(&mut self, command: &str, handler: WindowMsgHandler);
}

pub struct NullWindow
{
}

impl Window for NullWindow
{
    fn update(&mut self) {

    }

    fn send_msg(&mut self, _msg: &str) {

    }

    fn register_msg_handler(&mut self, _command: &str, _handler: fn(&str) -> bool) {

    }
}

pub struct ExternalWindow
{
    pub update_fn: ExternalWindowUpdateFn,
    pub receive_fn: ExternalWindowReceiveMsgFn,
    pub deliver_fn: ExternalWindowDeliverMsgFn,
}

impl ExternalWindow
{
    pub fn new() -> Self
    {
        ExternalWindow
        {
            update_fn: external_update_fn_stub,
            receive_fn: external_u8_fn_stub,
            deliver_fn: external_u8_fn_stub
        }
    }
}

impl Window for ExternalWindow
{
    fn update(&mut self) {
        (self.update_fn)();
    }

    fn send_msg(&mut self, msg: &str) {
        unimplemented!()
    }

    fn register_msg_handler(&mut self, command: &str, handler: fn(&str) -> bool) {
        unimplemented!()
    }
}

fn external_update_fn_stub()
{
}

fn external_u8_fn_stub(_: *const u8)
{
}