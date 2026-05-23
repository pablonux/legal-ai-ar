//@ts-nocheck
import { DropdownButton, ItemDataType, ValueType, Button } from '@appkit4/react-components';
import React, { useState } from 'react';

interface LanguageProps {
    className?: string;
    style?: React.CSSProperties;
    value?: string;
    data?: ItemDataType[];
    onSelect?: (value: string, item: ItemDataType) => void;
}


const Language = React.forwardRef<HTMLElement, LanguageProps>((props, ref) => {

    const {
        value: valueProps,
        data = [],
        onSelect,
        ...restProps
    } = props;

    const [value, setValue] = useState(valueProps);

    const handleLangaugeChange = (value: ValueType, item: ItemDataType, event: React.SyntheticEvent) => {
        setValue(value);
        onSelect?.(value as string, item);
    }
    
    return (
        <DropdownButton
            compact
            kind='text' 
            splitButton={false}
            data={data}
            onSelect={handleLangaugeChange}
            // customTriggerNode
        >
            <span className="Appkit4-icon icon-globe-outline"></span>
            {data.find((item: any) => item.value === value)?.label}
            {/* <Button kind='text' compact className="ap-login-form-compact-button">
                <div className='language-container'>
                    <div className="globe-icon-outline"></div>
                    
                    <div className='language-trigger'>
                        <span className='language-text'>
                            {data.find((item: any) => item.value === value)?.label}
                        </span>
                        <span className={classNames('down-chevron-outline', {'rotate': visible})}></span>
                    </div>
                </div>
            </Button> */}
            
        </DropdownButton>
    );
})

export default Language;

