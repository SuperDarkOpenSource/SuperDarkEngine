
use std::ffi::{CStr, CString};
use std::ptr::null;
use serde_json::Value;
use std::collections::HashMap;

pub type WindowMsgHandler = Box<dyn Fn(&str)>;

pub type ExternalWindowUpdateFn = fn();
pub type ExternalWindowReceiveMsgFn = fn() -> *const i8;
pub type ExternalWindowDeliverMsgFn = fn(*const i8);

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

    fn register_msg_handler(&mut self, _command: &str, _handler: WindowMsgHandler) {

    }
}

pub struct ExternalWindow
{
    pub update_fn: ExternalWindowUpdateFn,
    pub receive_fn: ExternalWindowReceiveMsgFn,
    pub deliver_fn: ExternalWindowDeliverMsgFn,
    command_map: HashMap<String, WindowMsgHandler>
}

impl ExternalWindow
{
    pub fn new() -> Self
    {
        ExternalWindow
        {
            update_fn: external_update_fn_stub,
            receive_fn: external_i8_receive_stub,
            deliver_fn: external_i8_fn_stub,
            command_map: HashMap::new(),
        }
    }

    fn poll_external_msg_queue(&mut self) {
        loop {
            let msg_raw: *const i8 = (&self.receive_fn)();

            if msg_raw == null() {
                break;
            }

            let msg = unsafe{CStr::from_ptr(msg_raw)};

            let json_msg: Value = serde_json::from_str(msg.to_str().unwrap()).unwrap();

            match json_msg {
                Value::Object(value) => {
                    if value.contains_key("command") && value.contains_key("payload") {
                        let command = value["command"].as_str().unwrap();
                        let payload = value["payload"].to_string();

                        self.handle_received_msg(command, payload.as_str());
                    }
                }
                _ => {}
            }
        }
    }

    fn handle_received_msg(&mut self, command: &str, payload: &str) {
        if self.command_map.contains_key(command) {
            (self.command_map[command])(payload);
        } else {
            println!("Command {} not found in ExternalWindow command handlers", command);
        }
    }
}

impl Window for ExternalWindow
{
    fn update(&mut self) {
        (&self.update_fn)();

        self.poll_external_msg_queue();
    }

    fn send_msg(&mut self, msg: &str) {
        // Convert to a C String
        let cstr = CString::new(msg).unwrap();

        // FFI to the ExternalWindow
        (&self.deliver_fn)(cstr.as_ptr());
    }

    fn register_msg_handler(&mut self, command: &str, handler: WindowMsgHandler) {
        self.command_map.insert(String::from(command), handler);
    }
}

fn external_update_fn_stub() {
}

fn external_i8_fn_stub(_: *const i8) { }

fn external_i8_receive_stub() -> *const i8 {
    null()
}