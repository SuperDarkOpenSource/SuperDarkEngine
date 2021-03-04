use crate::core::window::{Window, NullWindow};

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
    do_next_frame: bool,
}

impl GameEngine {

    pub fn new(create_info: Box<GameEngineCreationInfo>) -> Self {
        let mut engine = GameEngine {
            create_info,
            do_next_frame: true
        };

        //engine.create_info.window.register_msg_handler("hello",  self::set_run_false);

        engine
    }

    pub fn run(&mut self) {

        let mut window = &mut self.create_info.window;

        loop {
            window.update();
        }
    }

    pub fn set_run_false(&mut self, _msg: &str) -> bool {
        self.do_next_frame = false;

        true
    }
}