use crate::core::window::{Window, NullWindow};
use std::cell::RefCell;
use std::rc::Rc;

pub struct GameEngineCreationInfo
{
    pub window: Box<dyn Window>,

}

impl GameEngineCreationInfo {
    pub fn new() -> Self {
        GameEngineCreationInfo {
            window: Box::new(NullWindow{})
        }
    }
}


pub struct GameEngine
{
    create_info: Box<GameEngineCreationInfo>,
    do_next_frame: Rc<RefCell<bool>>,
}

impl GameEngine {

    pub fn new(create_info: Box<GameEngineCreationInfo>) -> Self {
        let mut do_next_frame = Rc::new(RefCell::new(true));

        let mut engine = GameEngine {
            create_info,
            do_next_frame: do_next_frame.clone()
        };

        engine.create_info.window.register_msg_handler("quit",  Box::new(move |payload: &str| {
            (*do_next_frame.borrow_mut()) = false;
        }));

        engine
    }

    pub fn run(&mut self) {

        let mut window = &mut self.create_info.window;

        while *(self.do_next_frame.borrow()) {

            window.update();

        }
    }
}