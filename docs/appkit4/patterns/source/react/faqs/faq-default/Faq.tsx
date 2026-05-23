import React from 'react';
import { Accordion, AccordionItem } from '@appkit4/react-components';
import classNames from 'classnames';
import './faq.scss';

export interface FaqProps {
    onKeydown?:  (event: React.KeyboardEvent<HTMLElement>, url?: string, params?: any) => void;
    onClickAccordion?: (event: React.MouseEvent<HTMLElement>, url?: string, params?: any) => void;
    onClickLink?: (event: React.MouseEvent<HTMLElement>, url?: string, params?: any) => void;
    className?: string;
    renderNode?: React.ReactNode;
    faqList?: FaqType[]
}

export interface FaqType {
    data?:FaqDataType[];
    category?:string;
}

export interface FaqDataType {
    title?:string;
    desc?:string;
}

const Faq = React.forwardRef<HTMLElement, FaqProps>((props: FaqProps, ref) => {
    const {
        onClickAccordion,
        onClickLink,
        className,
        faqList
    } = props;
    
    return (
        <div className={classNames('ap-pattern-faq-list', className)}>
            {
                faqList?.map((faq: FaqType, index: number) => {
                    return <div className="ap-pattern-faq-section" key={index}>
                        <div className="ap-pattern-faq-subtitle anchor-target" id={faq.category?.replace(/\s|\&/g,'').toLowerCase()}>{faq.category}</div>
                        <div className="ap-pattern-faq-content">
                            <Accordion multiple={false} onClick={onClickAccordion} >
                                {
                                    faq.data?.map((item: FaqDataType, index: number) => {
                                        return <AccordionItem title={item.title} itemKey={index.toString()} key={index}>
                                            <span className="ap-accordion-text" onClick={onClickLink} dangerouslySetInnerHTML={{ __html: item.desc||'' }}></span>
                                        </AccordionItem>
                                    })
                                }
                            </Accordion>
                        </div>
                    </div>
                })
            }
        </div>
    )
})

export default Faq