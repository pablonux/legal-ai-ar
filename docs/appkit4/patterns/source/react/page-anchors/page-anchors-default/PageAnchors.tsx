import React from 'react';
import classNames from 'classnames';
import './page-anchors.scss';

export interface PageAnchorsProps {
    onKeydown?:  (event: React.KeyboardEvent<HTMLElement>, url?: string, params?: any) => void;
    onAnchorClick?: (name?: string) => void;
    className?: string;
    renderNode?: React.ReactNode;
    anchorLeft?: number;
    anchorList?: AnchorType[];
    showAnchors?: boolean;
    enableHref?: boolean;
    showHeading?:boolean;
    headingText?:string;
    headingIcon?:string;
}
export interface AnchorType {
    value?:string;
    selected?:boolean;
    externalLink?:boolean;
    externalLinkIcon?:string;
}

const PageAnchors = React.forwardRef<HTMLElement, PageAnchorsProps>((props: PageAnchorsProps, ref) => {
    const {
        onAnchorClick,
        anchorLeft,
        anchorList,
        className,
        showAnchors = true,
        enableHref = true,
        showHeading = true,
        headingText = 'Heading',
        headingIcon = 'Appkit4-icon icon-open-in-new-outline'
    } = props;

    const isAllExternal = anchorList?.every(item => item.externalLink === true);

    const formatUrl = (url?: string, toLowercase = true) => {
        return url?.toLowerCase().replace(/\s+|\&+/g,'');
    }

    const onKeyDown = (event: React.KeyboardEvent<HTMLElement>, value?: string) => {
        const { key } = event;
        if (key === 'Enter') {
            onAnchorClick?.(value)
        }
    }

    const headingFunc = () => {
        return showHeading? <div tabIndex={0} className='ap-pattern-page-anchor-heading'>{isAllExternal&&<span className={headingIcon}></span>}{headingText}</div> :
        (isAllExternal? <div tabIndex={0} className='ap-pattern-page-anchor-heading'>{isAllExternal&&<span className={headingIcon}></span>}</div> : null);
    }

    return (
        <nav ref={ref} className={classNames('ap-pattern-page-anchor', className, {'hidden':!showAnchors})} style={{left: `${anchorLeft}px`}}>
                        {headingFunc()}
                        <ul>
                            {
                                anchorList?.map(
                                    (anchor: AnchorType, index: number) => {
                                        const hrefValue = window.location.href.indexOf("#") > -1? window.location.href.substring(0, window.location.href.indexOf("#")) : window.location.href;
                                        const hrefProp = enableHref? {href: encodeURI(hrefValue + '#' + formatUrl(anchor.value))} : null;
                                        return <li key={index} onClick={(event) => onAnchorClick?.(anchor.value)} onKeyDown={(event)=>onKeyDown(event, anchor.value)}>
                                                    <a tabIndex={0} role="link" aria-selected={anchor.selected} {...hrefProp} id={'anchor-' + formatUrl(anchor.value)}
                                                        className={classNames("anchor-trigger", { 'selected': anchor.selected })}>
                                                        {anchor.externalLink && <span className={anchor.externalLinkIcon||"Appkit4-icon icon-open-in-new-outline"}></span>}
                                                        {anchor.value}
                                                    </a>
                                        </li>
                                    }
                                )
                            }
                        </ul>
                    </nav>
    )
})

export default PageAnchors