import React from 'react';
import './card.scss';
import classNames from 'classnames';
export interface CardItemType {
    name?: string;
    description?: string;
    dateTextIcon?: string;
    dateText?: string;
    link?: string;
    params?: any;
    src?: string; // If use src here, there is no need to use like content: url("./placeholder.svg") in css to show the image
    prefixIconClass?: string;
    timeTextIcon?: string;
    timeText?: string;
}
export interface CardProps {
  item?: CardItemType;
  onKeydown?:  (event: React.KeyboardEvent<HTMLElement>, url?: string, params?: any) => void;
  onClick?:  (event: React.MouseEvent<HTMLElement>, url?: string, params?: any) => void;
  className?: string;
}
const CardPattern = React.forwardRef<HTMLElement, CardProps>((props: CardProps, ref) => {
    const {
        item,
        onClick,
        onKeydown,
        className
    } = props;
    return (
        <div className={classNames('ap-pattern-card', className)} role="link" tabIndex={0}
        onClick={(event) => onClick?.(event, item?.link, item?.params)}
        onKeyDown={(event) => onKeydown?.(event, item?.link, item?.params)}>
            <div className="ap-pattern-card-according component-thumb">
                {item?.src? <img alt="" src={item?.src}/> : <img alt=""/>}
            </div>
            <div className="ap-pattern-card-description">
            <div className='ap-pattern-card-head'>
            {item?.prefixIconClass && <span className={classNames("prefixIcon", item?.prefixIconClass)} aria-label={item?.name}></span>}
            <span className="ap-pattern-card-name">
                {item?.name}
            </span>
            </div>
            <p className="ap-pattern-card-desc">
                {item?.description}
            </p>
            </div>
            <div className="card-pattern-footer">
                    <span className="component-footer-date">
                        {item?.dateTextIcon && <span className={classNames("prefixIcon", item?.dateTextIcon)} aria-label={item?.dateText}></span>}
                        <span className="footer-text">{item?.dateText}</span>
                    </span>
                    <span className="component-footer-time">
                        {item?.timeTextIcon && <span className={classNames("prefixIcon", item?.timeTextIcon)} aria-label={item?.timeText}></span>}
                        <span className="footer-text">{item?.timeText}</span>
                    </span>
            </div>
        </div>
  )
})

export default CardPattern

