import React from 'react';
import PageAnchors from './PageAnchors';
const anchorArray = [
    {
        value: 'General',
        selected: true
    },
    {
        value: "Appkit Access"
    },
    {
        value: "Compliance & accessibility"
    },
    {
        value: "Licences"
    },
    {
        value: "Developers"
    },
    {
        value: "Designers"
    },
    {
        value: "About Appkit"
    }
];
const PageAnchorsDefault = React.forwardRef<HTMLElement, any>((props: any, ref) => {
    const [anchorList, setAnchorList] = React.useState(anchorArray.map((item, index) => {
        return { ...item, selected: index === 0 ? true : false };
    }));
    const onAnchorClick = (value?: string) => {
        setAnchorList(anchorArray.map((item, index) => {
            if (item.value?.toLowerCase() === value?.toLowerCase())
                return { ...item, selected: true };
            else return { ...item, selected: false }
        }));
    }
    return <PageAnchors headingText='Jump to' onAnchorClick={onAnchorClick} enableHref={false} anchorList={anchorList}></PageAnchors>;
})

export default PageAnchorsDefault
