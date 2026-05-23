import React from 'react';
import Faq from './Faq';

const faqs = [
    {
        data: [
            {
                title: "Lorem ipsum dolor sit amet",
                desc: "Lorem ipsum dolor sit amet"
            },
            {
                title: "Lorem ipsum dolor sit amet",
                desc: "Lorem ipsum dolor sit amet"
            },
            {
                title: "Lorem ipsum dolor sit amet",
                desc: "Lorem ipsum dolor sit amet"
            },
            {
                title: "Lorem ipsum dolor sit amet",
                desc: "Lorem ipsum dolor sit amet"
            },
            {
                title: "Lorem ipsum dolor sit amet",
                desc: "Lorem ipsum dolor sit amet"
            }
        ],
        category: "Appkit Access"
    }
];
const FaqDefault = React.forwardRef<HTMLElement, any>((props: any, ref) => {
    return <Faq faqList={faqs}></Faq>;
})

export default FaqDefault