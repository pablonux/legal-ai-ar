import React, { useState, MouseEvent, KeyboardEvent } from 'react';
import { Button } from '@appkit4/react-components';
import classNames from 'classnames';
import FloatButton from './FloatButton';
import useClickOutside from './useClickOutside';
import './multiple-action-menu.scss';

export const KEY_VALUES = {
    // Navigation Keys
    LEFT: 'ArrowLeft',
    UP: 'ArrowUp',
    RIGHT: 'ArrowRight',
    DOWN: 'ArrowDown',
    END: 'End',
    HOME: 'Home',
    PAGE_DOWN: 'PageDown',
    PAGE_UP: 'PageUp',
  
    // Whitespace Keys
    ENTER: 'Enter',
    TAB: 'Tab',
    SPACE: ' ',
  
    // Editing Keys
    BACKSPACE: 'Backspace',
    DELETE: 'Delete',
    COMMA: ',',
  
    // UI Keys
    ESC: 'Escape',
    SHIFT: 'Shift'
  };

export interface MultipleActionMenuProps {
    className?: string;
}

const MultipleActionMenu = React.forwardRef<HTMLElement, MultipleActionMenuProps>((props: MultipleActionMenuProps, ref) => {
    const {
        className
    } = props;


    const actionMenuRef = React.useRef(null);
    const triggerRef = React.useRef(null);
    const containerRef = React.useRef(null);

    const [open, setOpen] = useState(false);
    const [keyboardOpen, setKeyboardOpen] = useState(false);

    const onTriggerButtonClick = (event: any) => {
        if (event.type === 'click') {
            setOpen(!open);
            setKeyboardOpen(false);
            if (triggerRef.current) {
                (triggerRef.current as HTMLElement).blur();
            }
            
        } else if (event.type === 'keydown') {
            setOpen(!open);
            setKeyboardOpen(true);
        }
      };

      const hideOnClickOutside = () => {
        if (open) {
            setOpen(!open);
            setKeyboardOpen(!keyboardOpen);
        }
      }

      useClickOutside({
        enabled: true,
        isOutside: event => {
          const eventSource = event.target;
    
          return (
            !(containerRef.current! as HTMLElement)?.contains(eventSource as HTMLElement)
          );
        },
        handle: hideOnClickOutside,
      });

      const onKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
        const { key } = event;
        const isDown = key === KEY_VALUES.DOWN;
        const isUp = key === KEY_VALUES.UP;
        const isEsc = key === KEY_VALUES.ESC;

        if (open) {
            if (isEsc) {
                setOpen(!open);
                setKeyboardOpen(!keyboardOpen);
                if (triggerRef.current) {
                    (triggerRef.current as HTMLElement).focus();
                }
            }
            if (isDown || isUp) {
                if (actionMenuRef.current) {
                    const buttons = (actionMenuRef.current as HTMLElement)?.querySelectorAll('.ap-float-btn');  
                    // Find the currently focused element  
                    const focusedElement = document.activeElement;  
                      
                    // Determine the index of the currently focused element  
                    let currentIndex = -1;  
                    buttons.forEach((button, index) => {  
                        if (button === focusedElement) {  
                            currentIndex = index;  
                        }  
                    });
              
                    // Calculate the next index
                    if (isDown) {  
                        let nextIndex = (currentIndex + 1) % buttons.length;  
                        (buttons[nextIndex] as HTMLElement).focus();  
                    } else if (isUp) {  
                        let nextIndex = (currentIndex - 1 + buttons.length) % buttons.length;
                        (buttons[nextIndex] as HTMLElement).focus();  
                    }
                }
            }
        }
      };

      React.useEffect(() => {
        
        if (open && keyboardOpen) {
            if (actionMenuRef.current) {
                const ele = (actionMenuRef.current as HTMLElement)?.querySelector('.ap-float-btn') as HTMLElement;
                ele?.focus();
            }
        }
      }, [open, keyboardOpen]);


    return (
        <div
            ref={containerRef}
            className='ap-action-menu-group'
            onKeyDown={onKeyDown}
        >
            {
                open ? (
                    <div className='ap-action-menu-group-wrap' ref={actionMenuRef}>
                        <FloatButton
                            type='rose'
                            tooltip={
                                <div className='ap-action-menu-tooltip-content'>
                                    <span className='title'>Ask Model Edge</span>
                                    <span className='subtitle'>Powered by GenAI</span>
                                </div>
                            }
                            icon={'icon-need-heep-bot-outline'}
                            aria-label='Ask Model Edge'
                        ></FloatButton>
                        <FloatButton
                            tooltip={
                                <div className='ap-action-menu-tooltip-content'>
                                    <span className='title'>Get Help</span>
                                    <span className='subtitle'>Browse our knowledge base</span>
                                </div>
                            }
                            kind='tertiary'
                            icon={'icon-information-outline'}
                            aria-label='Get Help'
                        >
                        </FloatButton>
                        <FloatButton
                            tooltip={
                                <div className='ap-action-menu-tooltip-content'>
                                    <span className='title'>{'[3rd option]'}</span>
                                    <span className='subtitle'>Placeholder to show scalability</span>
                                </div>
                            }
                            icon={'icon-placeholder-outline'}
                            aria-label='[3rd option]'
                        ></FloatButton>
                    </div>
                ) : null
            }
            <FloatButton
                ref={triggerRef}
                type='inverse'
                icon={"icon-plus-outline"}
                className={
                    classNames('ap-action-menu-group-trigger', {
                        'open': open
                    })
                }
                aria-label={'Open action menu'}
                aria-expanded={open}
                onClick={onTriggerButtonClick}
            ></FloatButton>
        </div>
    );

})

export default MultipleActionMenu;