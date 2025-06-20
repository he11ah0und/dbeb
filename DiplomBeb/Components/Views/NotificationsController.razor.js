function outAnimation(){
    const not = document.querySelector('.window');
    not.style.animation = 'fadeOutSlideUp 0.6s ease-in forwards';
}

function inAnimation(){
    const not = document.querySelector('.window');
    not.style.animation = '';
}