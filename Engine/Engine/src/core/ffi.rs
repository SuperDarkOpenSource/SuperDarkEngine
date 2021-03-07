use crate::core::window::{ExternalWindow, ExternalWindowUpdateFn, ExternalWindowReceiveMsgFn, ExternalWindowDeliverMsgFn};
use crate::core::game_engine::{GameEngineCreationInfo, GameEngine};

#[no_mangle]
extern fn externalwindow_create() -> *mut ExternalWindow {
    let new_box = Box::new(ExternalWindow::new());

    Box::into_raw(new_box)
}

#[no_mangle]
extern fn externalwindow_set_update_fn(external_window: *mut ExternalWindow, update_fn: ExternalWindowUpdateFn) {
    unsafe {
        (*external_window).update_fn = update_fn;
    }
}

#[no_mangle]
extern fn externalwindow_set_receive_fn(external_window: *mut ExternalWindow, receive_fn: ExternalWindowReceiveMsgFn) {
    unsafe {
        (*external_window).receive_fn = receive_fn;
    }
}

#[no_mangle]
extern fn externalwindow_set_deliver_fn(external_window: *mut ExternalWindow, deliver_fn: ExternalWindowDeliverMsgFn) {
    unsafe {
        (*external_window).deliver_fn = deliver_fn;
    }
}

#[no_mangle]
extern fn gameenginecreationinfo_create() -> *mut GameEngineCreationInfo {
    Box::into_raw(Box::new(GameEngineCreationInfo::new()))
}

#[no_mangle]
extern fn gameenginecreationinfo_set_externalwindow(create_info: *mut GameEngineCreationInfo, externalwindow: *mut ExternalWindow) {
    unsafe {
        (*create_info).window = Box::from_raw(externalwindow)
    }
}

#[no_mangle]
extern fn gameengine_create(create_info: *mut GameEngineCreationInfo) -> *mut GameEngine {
    Box::into_raw(Box::new(GameEngine::new(unsafe {
        Box::from_raw(create_info)
    })))
}

#[no_mangle]
extern fn gameengine_run(game_engine_raw: *mut GameEngine) {
    let mut game_engine = unsafe {
        Box::from_raw(game_engine_raw)
    };

    game_engine.run();
}
