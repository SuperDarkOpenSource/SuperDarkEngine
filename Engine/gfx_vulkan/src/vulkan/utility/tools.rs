use std::ffi::CStr;
use std::os::raw::c_char;


pub fn c_char_array_to_string(char_array: &[c_char]) -> String
{
    let raw_string = unsafe {
        let pointer = char_array.as_ptr();
        CStr::from_ptr(pointer)
    };

    raw_string.to_str()
        .expect("Failed to convert char array to string")
        .to_owned()
}